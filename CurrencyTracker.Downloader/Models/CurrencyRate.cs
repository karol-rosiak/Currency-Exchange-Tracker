using System.Text.Json.Serialization;

namespace CurrencyTracker.Downloader.Models
{
    public class CurrencyRate
    {
        [JsonPropertyName("no")]
        public string? No { get; set; }

        [JsonPropertyName("effectiveDate")]
        public string? EffectiveDate { get; set; }

        [JsonPropertyName("bid")]
        public decimal Bid { get; set; }

        [JsonPropertyName("ask")]
        public decimal Ask { get; set; }
    }
}
