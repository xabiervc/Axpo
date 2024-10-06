namespace PowerTrade.Services
{
    public interface IScheduler
    {
        Task StartAsync(string timeZoneId);
    }
}