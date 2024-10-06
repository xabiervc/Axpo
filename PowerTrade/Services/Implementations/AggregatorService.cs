using PowerTrade.Extensions;
using PowerTrade.Models;

namespace PowerTrade.Services.Implementations
{
    public class AggregatorService : IAggregatorService
    {
        public IEnumerable<AggregatedTradeModel> AggregateTrades(IEnumerable<Axpo.PowerTrade> trades)
        {
            var aggregatedData = new Dictionary<DateTime, double>();

            foreach (var trade in trades)
            {
                foreach (var period in trade.Periods)
                {
                    var dateTime = period.GetStartDate(trade.Date);
                    if (aggregatedData.ContainsKey(dateTime))
                    {
                        aggregatedData[dateTime] += period.Volume;
                    }
                    else
                    {
                        aggregatedData[dateTime] = period.Volume;
                    }
                }
            }

            return aggregatedData.Select(x => new AggregatedTradeModel { DateTime = x.Key, Volume = x.Value });
        }
    }
}