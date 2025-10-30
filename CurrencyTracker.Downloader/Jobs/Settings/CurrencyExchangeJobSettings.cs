namespace CurrencyTracker.Downloader.Jobs.Settings
{
    public class CurrencyExchangeJobSettings
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int MaxRetries { get; set; }
        public int RetryDelayHours { get; set; }
    }
}

