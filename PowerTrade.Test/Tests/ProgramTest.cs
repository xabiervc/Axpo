using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using PowerTrade.Models;
using PowerTrade.Services;
using PowerTrade.Services.Implementations;

namespace PowerTrade.Test.Tests
{
    [TestFixture]
    public class ProgramTest
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IServiceCollection> _mockServiceCollection;
        private Mock<IServiceProvider> _mockServiceProvider;
        private Mock<ILogger<Program>> _mockLogger;
        private ConfigModel _configModel;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockServiceCollection = new Mock<IServiceCollection>();
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockLogger = new Mock<ILogger<Program>>();

            _configModel = new ConfigModel
            {
                DefaultTimeZone = "Europe/Berlin"
            };

            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x.Value).Returns("Europe/Berlin");

            _mockConfiguration.Setup(x => x.GetSection(It.IsAny<string>())).Returns(mockConfigSection.Object);
        }

        [Test]
        public void TestConfigurationSetup()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddCommandLine(new string[] { });
                });

            var host = hostBuilder.Build();
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            Assert.IsNotNull(configuration);
        }

        [Test]
        public void TestServiceRegistration()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.Configure<ConfigModel>(context.Configuration);
                    services.AddSingleton<IConfiguration>(context.Configuration);
                    services.AddSingleton<Axpo.PowerService>();
                    services.AddSingleton<ITradeService, TradeService>();
                    services.AddSingleton<IAggregatorService, AggregatorService>();
                    services.AddSingleton<ICsvGeneratorService, CsvGeneratorService>();
                    services.AddSingleton<IScheduler, Scheduler>();
                    services.AddLogging(x => x.AddConsole());
                });

            var host = hostBuilder.Build();
            var scheduler = host.Services.GetRequiredService<IScheduler>();

            Assert.IsNotNull(scheduler);
        }

        [Test]
        public void TestTimeZoneInput_Valid()
        {
            var inputTimeZone = "Europe/Istanbul";
            var timeZoneId = _configModel.DefaultTimeZone;

            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(inputTimeZone);
                timeZoneId = inputTimeZone;
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
            {
                _mockLogger.Object.LogWarning("The provided time zone '{TimeZone}' is not valid. Using the default time zone 'Europe/Berlin'.", inputTimeZone);
                _mockLogger.Object.LogWarning(ex.Message);
            }

            Assert.AreEqual("Europe/Istanbul", timeZoneId);
        }

        [Test]
        public void TestTimeZoneInput_Invalid()
        {
            var inputTimeZone = "Invalid/TimeZone";
            var timeZoneId = _configModel.DefaultTimeZone;

            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(inputTimeZone);
                timeZoneId = inputTimeZone;
            }
            catch (Exception ex) when (ex is TimeZoneNotFoundException || ex is InvalidTimeZoneException)
            {
                _mockLogger.Object.LogWarning("The provided time zone '{TimeZone}' is not valid. Using the default time zone 'Europe/Berlin'.", inputTimeZone);
                _mockLogger.Object.LogWarning(ex.Message);
            }

            Assert.AreEqual("Europe/Berlin", timeZoneId);
        }

        [Test]
        public async Task TestSchedulerStartAsync()
        {
            var mockScheduler = new Mock<IScheduler>();
            mockScheduler.Setup(s => s.StartAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(mockScheduler.Object);
                });

            var host = hostBuilder.Build();
            var scheduler = host.Services.GetRequiredService<IScheduler>();

            await scheduler.StartAsync("Europe/Berlin");

            mockScheduler.Verify(s => s.StartAsync("Europe/Berlin"), Times.Once);
        }
    }
}