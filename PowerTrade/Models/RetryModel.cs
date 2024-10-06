namespace PowerTrade.Models
{
    public record RetryModel
    {
        public int Maximum { get; set; }

        public int DelayMiliseconds { get; set; }
    }
}