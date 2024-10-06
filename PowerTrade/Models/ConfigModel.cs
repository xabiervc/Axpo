namespace PowerTrade.Models
{
    public record ConfigModel
    {
        public FileModel File { get; set; }

        public int IntervalMinutes { get; set; }

        public RetryModel Retry { get; set; }

        public string DefaultTimeZone { get; set; }

        public string TimeZone { get; set; }

        public TimeZoneInfo TimeZoneInfo => TimeZoneInfo.FindSystemTimeZoneById(TimeZone);

        public DateTime NextReport { get; set; }
    }
}