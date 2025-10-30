namespace CurrencyTracker.Data.Cache
{
    public static class RedisSettings
    {
        private const string ExchangeRateKeyTemplate = "object:{0}:{1}:{2}";
        private const string CurrencyKeyTemplate = "currency:{0}";
        private const string CurrenciesKeyTemplate = "currencies";

        public static string GetExchangeRateKey(string? baseCode, string? targetCode, DateOnly? date)
            => string.Format(ExchangeRateKeyTemplate, baseCode, targetCode, $"{date?.Year:D4}-{date?.Month:D2}-{date?.Day:D2}");

        public static string GetCurrencyKey(string? code) => string.Format(CurrencyKeyTemplate, code);

        public static string GetCurrenciesKey() => CurrenciesKeyTemplate;

        public static TimeSpan GetCacheTimeSpan => TimeSpan.FromDays(1);
    }

}