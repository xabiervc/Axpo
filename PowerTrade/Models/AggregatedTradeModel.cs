namespace PowerTrade.Models
{
    public record AggregatedTradeModel
    {
        public DateTime DateTime { get; set; }

        public double Volume { get; set; }
    }
}