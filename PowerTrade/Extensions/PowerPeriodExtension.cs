namespace PowerTrade.Extensions
{
    public static class PowerPeriodExtension
    {
        public static DateTime GetStartDate(this Axpo.PowerPeriod period, DateTime tradeDate)
        {
            return tradeDate.Date.AddHours(period.Period - 1).ToUniversalTime();
        }
    }
}