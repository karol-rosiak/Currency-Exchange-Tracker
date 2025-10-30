namespace CurrencyTracker.Models
{
    public class CurrencyExchangeRate
    {
        public string? Currency  { get; set; }

        public string? Code { get; set; }

        public decimal? Ask { get; set; }

        public decimal? Bid { get; set; }

        public DateOnly? ExchangeDate { get; set; }

    }
}
