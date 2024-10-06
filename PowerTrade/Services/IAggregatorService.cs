using PowerTrade.Models;

namespace PowerTrade.Services
{
    public interface IAggregatorService
    {
        IEnumerable<AggregatedTradeModel> AggregateTrades(IEnumerable<Axpo.PowerTrade> trades);
    }
}