namespace PowerTrade.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime ToLocalDateTime(this DateTime dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
        }

        public static DateTimeOffset ToLocalDateTime(this DateTimeOffset dateTime, TimeZoneInfo timeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime.UtcDateTime, timeZone);
        }
    }
}