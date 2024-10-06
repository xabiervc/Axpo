using NUnit.Framework;
using PowerTrade.Models;
using PowerTrade.Services.Implementations;
using System;
using System.Collections.Generic;

namespace PowerTrade.Tests
{
    [TestFixture]
    public static class AggregatorServiceFixture
    {
        public static IEnumerable<Axpo.PowerTrade> GetNoTrades() => new List<Axpo.PowerTrade>()
        {
        };

        public static IEnumerable<Axpo.PowerTrade> GetOkTrades()
        {
            var trades = new List<Axpo.PowerTrade>();
            var trade1 = Axpo.PowerTrade.Create(DateTime.Now, 2);
            var trade2 = Axpo.PowerTrade.Create(DateTime.Now, 2);
            trade1.Periods[0].SetVolume(100);
            trade1.Periods[1].SetVolume(150);
            trade2.Periods[0].SetVolume(150);
            trade2.Periods[1].SetVolume(250);
            trades.Add(trade1);
            trades.Add(trade2);
            return trades;
        }

        public static IEnumerable<Axpo.PowerTrade> GetDifferentDayTrades()
        {
            var trades = new List<Axpo.PowerTrade>();
            var trade1 = Axpo.PowerTrade.Create(DateTime.Now, 1);
            var trade2 = Axpo.PowerTrade.Create(DateTime.Now.AddDays(1), 1);
            trade1.Periods[0].SetVolume(100);
            trade2.Periods[0].SetVolume(200);
            trades.Add(trade1);
            trades.Add(trade2);
            return trades;
        }

        public static IEnumerable<Axpo.PowerTrade> GetNoPeriodTrades() => new List<Axpo.PowerTrade>()
        {
            Axpo.PowerTrade.Create(DateTime.Now, 0),
            Axpo.PowerTrade.Create(DateTime.Now.AddDays(1), 0)
        };
    }
}
