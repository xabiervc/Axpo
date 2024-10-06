using Microsoft.Extensions.Options;
using Moq;
using PowerTrade.Extensions;
using PowerTrade.Models;
using PowerTrade.Services.Implementations;
using System.Globalization;

namespace PowerTrade.Test.Tests
{
    [TestFixture]
    public class CsvGeneratorServiceTests
    {
        private Mock<IOptions<ConfigModel>> _configMock;
        private CsvGeneratorService _csvGeneratorService;
        private ConfigModel _configModel;
        private ConfigModel _config;

        [SetUp]
        public void SetUp()
        {
            _configModel = new ConfigModel
            {
                TimeZone = "Europe/Berlin",
                File = new FileModel
                {
                    Path = "C:\\PowerTradeReportsTests",
                    Name = "Test_{0}_{1}.csv"
                }
            };

            _configMock = new Mock<IOptions<ConfigModel>>();
            _configMock.Setup(x => x.Value).Returns(_configModel);

            _csvGeneratorService = new CsvGeneratorService(_configMock.Object);

            _config = _configMock.Object.Value;
        }

        [Test]
        public void Generate_ShouldCreateDirectoryIfNotExists()
        {
            // Arrange
            var data = new List<AggregatedTradeModel>
            {
                new AggregatedTradeModel { DateTime = DateTime.UtcNow, Volume = 100 }
            };
            var filePath = _csvGeneratorService.GetFullFilePathName();
            var directoryPath = Path.GetDirectoryName(filePath);

            // Act
            _csvGeneratorService.Generate(data);

            // Assert
            Assert.IsTrue(Directory.Exists(directoryPath));
        }

        [Test]
        public void Generate_ShouldWriteCorrectDataToFile()
        {
            // Arrange
            var data = new List<AggregatedTradeModel>
            {
                new AggregatedTradeModel { DateTime = DateTime.UtcNow, Volume = 100 }
            };

            // Act
            _csvGeneratorService.Generate(data);
            var filePath = _csvGeneratorService.GetFullFilePathName();

            // Assert
            var lines = File.ReadAllLines(filePath);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("Datetime,Volume", lines[0]);
            Assert.AreEqual($"{data[0].DateTime.ToLocalDateTime(_config.TimeZoneInfo):yyyy-MM-ddTHH:mm:ssZ},{data[0].Volume.ToString(CultureInfo.InvariantCulture)}", lines[1]);
        }

        [Test]
        public void Generate_ShouldHandleEmptyData()
        {
            // Arrange
            var data = new List<AggregatedTradeModel>();

            // Act
            _csvGeneratorService.Generate(data);
            var filePath = _csvGeneratorService.GetFullFilePathName();

            // Assert
            var lines = File.ReadAllLines(filePath);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("Datetime,Volume", lines[0]);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Directory.Exists(_config.File.Path))
            {
                Directory.Delete(_config.File.Path, true);
            }
        }
    }
}