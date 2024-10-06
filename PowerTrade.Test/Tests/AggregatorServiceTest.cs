using NUnit.Framework;
using Moq;
using PowerTrade.Models;
using PowerTrade.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using PowerTrade.Tests;

namespace PowerTrade.Test.Tests
{
    [TestFixture]
    public class AggregatorServiceTest
    {
        private AggregatorService _aggregatorService;

        [SetUp]
        public void SetUp()
        {
            _aggregatorService = new AggregatorService();
        }

        [Test]
        public void AggregateTrades_ShouldReturnEmpty_WhenNoTradesProvided()
        {
            // Arrange
            var trades = AggregatorServiceFixture.GetNoTrades();

            // Act
            var result = _aggregatorService.AggregateTrades(trades);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void AggregateTrades_ShouldAggregateCorrectly()
        {
            // Arrange
            var trades = AggregatorServiceFixture.GetOkTrades();

            // Act
            var result = _aggregatorService.AggregateTrades(trades).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(250, result[0].Volume);
            Assert.AreEqual(400, result[1].Volume);
        }

        [Test]
        public void AggregateTrades_ShouldHandleMultipleDates()
        {
            // Arrange
            var trades = AggregatorServiceFixture.GetDifferentDayTrades();

            // Act
            var result = _aggregatorService.AggregateTrades(trades).ToList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(100, result[0].Volume);
            Assert.AreEqual(DateTime.Now.AddDays(-1).ToString("d"), result[0].DateTime.ToString("d"));
            Assert.AreEqual(200, result[1].Volume);
            Assert.AreEqual(DateTime.Now.ToString("d"), result[1].DateTime.ToString("d"));
        }

        [Test]
        public void AggregateTrades_ShouldHandleEmptyPeriods()
        {
            // Arrange
            var trades = AggregatorServiceFixture.GetNoPeriodTrades();

            // Act
            var result = _aggregatorService.AggregateTrades(trades);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
    }
}