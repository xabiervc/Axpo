using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PowerTrade.Models;
using PowerTrade.Services;
using PowerTrade.Services.Implementations;

namespace PowerTrade.Test.Tests
{
    [TestFixture]
    public class SchedulerTest
    {
        private Mock<ILogger<Scheduler>> _mockLogger;
        private Mock<ITradeService> _mockTradeService;
        private Mock<IAggregatorService> _mockAggregatorService;
        private Mock<ICsvGeneratorService> _mockCsvGenerator;
        private Mock<IOptions<ConfigModel>> _mockConfig;
        private Scheduler _scheduler;
        private ConfigModel _configModel;
        private List<string> _logMessages;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<Scheduler>>();
            _mockTradeService = new Mock<ITradeService>();
            _mockAggregatorService = new Mock<IAggregatorService>();
            _mockCsvGenerator = new Mock<ICsvGeneratorService>();
            _mockConfig = new Mock<IOptions<ConfigModel>>();

            _configModel = new ConfigModel
            {
                Retry = new RetryModel { Maximum = 3, DelayMiliseconds = 1000 },
                IntervalMinutes = 60,
                TimeZone = "Europe/Berlin",
                NextReport = DateTime.UtcNow.AddMinutes(60)
            };

            _mockConfig.Setup(x => x.Value).Returns(_configModel);

            _scheduler = new Scheduler(_mockLogger.Object, _mockTradeService.Object, _mockAggregatorService.Object, _mockCsvGenerator.Object, _mockConfig.Object);
        }

        [Test]
        public async Task StartAsync_ShouldLogStartAndScheduleNextReport()
        {
            // Arrange
            var timeZoneId = "Europe/Istanbul";

            // Act
            await _scheduler.StartAsync(timeZoneId);

            // Assert
            var logMessages = _mockLogger.Invocations
                .Where(x => x.Method.Name == nameof(ILogger.Log))
                .Select(x => x.Arguments[2].ToString())
                .ToList();

            Assert.IsTrue(logMessages.Contains("Scheduler started."));
            Assert.IsTrue(logMessages.Any(x => x.Contains("Next report scheduled at:")));
        }

        [Test]
        public async Task GenerateReport_ShouldGenerateReportSuccessfully()
        {
            // Arrange
            var timeZoneId = "Europe/London";
            var trades = new List<Axpo.PowerTrade> { };
            _mockTradeService.Setup(x => x.GetTradesAsync(It.IsAny<DateTime>())).ReturnsAsync(trades);
            _mockAggregatorService.Setup(x => x.AggregateTrades(trades)).Returns(new List<AggregatedTradeModel>());
            _mockCsvGenerator.Setup(x => x.Generate(It.IsAny<IEnumerable<AggregatedTradeModel>>()));

            // Act
            await _scheduler.StartAsync(timeZoneId);

            // Assert
            _mockCsvGenerator.Verify(x => x.Generate(It.IsAny<IEnumerable<AggregatedTradeModel>>()), Times.Once);
        }

        [Test]
        public async Task GenerateReport_ShouldRetryOnFailure()
        {
            // Arrange
            _mockTradeService.Setup(x => x.GetTradesAsync(It.IsAny<DateTime>())).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _scheduler.StartAsync("UTC"));

            var logMessages = _mockLogger.Invocations
                .Where(x => x.Method.Name == nameof(ILogger.Log))
                .Select(x => x.Arguments[2].ToString())
                .ToList();

            Assert.AreEqual(_configModel.Retry.Maximum, logMessages.Count(x => x.Contains("An error occurred while generating the report")));
        }
    }
}