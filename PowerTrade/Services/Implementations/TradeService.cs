namespace PowerTrade.Services.Implementations
{
    public class TradeService(Axpo.PowerService powerService) : ITradeService
    {
        public IEnumerable<Axpo.PowerTrade> GetTrades(DateTime date)
        {
            return powerService.GetTrades(date);
        }

        public async Task<IEnumerable<Axpo.PowerTrade>> GetTradesAsync(DateTime date)
        {
            return await powerService.GetTradesAsync(date);
        }
    }
}