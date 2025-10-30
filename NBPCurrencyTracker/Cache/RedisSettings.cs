namespace CurrencyTracker.Cache
{
    public static class RedisSettings
    {
        private const string ObjectKeyTemplate = "object:{0}:{1}"; 

        public static string GetObjectKey(string? code, DateOnly? date)
            => string.Format(ObjectKeyTemplate, code, $"{date?.Year:D4}-{date?.Month:D2}-{date?.Day:D2}");

        public static TimeSpan GetCacheTimeSpan => TimeSpan.FromDays(1);
    }

}