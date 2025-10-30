namespace CurrencyTracker.Downloader.Models
{
    public class CurrencyExchangeRateNBP
    {
        public string? Table { get; set; }
        public string? Currency { get; set; }
        public string? Code { get; set; }
        public CurrencyRate[]? Rates { get; set; }
    }
}
