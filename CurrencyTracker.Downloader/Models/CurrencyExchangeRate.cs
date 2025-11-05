using System.Text.Json.Serialization;

namespace CurrencyTracker.Downloader.Models
{
    public class CurrencyExchangeRateNBP
    {
        [JsonPropertyName("table")]
        public string? Table { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("rates")]
        public CurrencyRate[]? Rates { get; set; }
    }
}
