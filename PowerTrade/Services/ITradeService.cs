namespace PowerTrade.Services
{
    public interface ITradeService
    {
        IEnumerable<Axpo.PowerTrade> GetTrades(DateTime date);

        Task<IEnumerable<Axpo.PowerTrade>> GetTradesAsync(DateTime date);
    }
}