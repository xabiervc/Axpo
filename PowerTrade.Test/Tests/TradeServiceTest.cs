using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PowerTrade.Services.Implementations;
using Axpo;

namespace PowerTrade.Tests
{
    public class TestPowerService : PowerService
    {
        public IEnumerable<Axpo.PowerTrade> GetTrades(DateTime date)
        {
            return new List<Axpo.PowerTrade> { };
        }

        public Task <IEnumerable<Axpo.PowerTrade>> GetTradesAsync(DateTime date)
        {
            return Task.FromResult<IEnumerable<Axpo.PowerTrade>>(new List<Axpo.PowerTrade> { });
        }
    }

    [TestFixture]
    public class TradeServiceTests
    {
        private TestPowerService _testPowerService;
        private TradeService _tradeService;

        [SetUp]
        public void SetUp()
        {
            _testPowerService = new TestPowerService();
            _tradeService = new TradeService(_testPowerService);
        }

        [Test]
        public void GetTrades_ShouldReturnTrades()
        {
            // Arrange
            var date = new DateTime(2024, 10, 6);

            // Act
            var result = _tradeService.GetTrades(date);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public async Task GetTradesAsync_ShouldReturnTrades()
        {
            // Arrange
            var date = new DateTime(2024, 10, 6);

            // Act
            var result = await _tradeService.GetTradesAsync(date);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }
    }
}