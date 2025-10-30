namespace CurrencyTracker.Downloader.Models
{
    public class CurrencyRate
    {
        public string? No { get; set; }
        public string? EffectiveDate { get; set; }
        public decimal? Bid { get; set; }
        public decimal? Ask { get; set; }
        public CurrencyRate[]? Rate  {get; set;}
    }
}
