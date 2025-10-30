//using Microsoft.Extensions.Caching.Distributed;
//using CurrencyTracker.Cache;
//using CurrencyTracker.Database.Repositories;
//using CurrencyTracker.Models;
//using System.Text.Json;

//namespace CurrencyTracker.Services
//{
//    public class CurrencyExchangeService
//    {
//        private readonly CurrencyExchangeRepository _currencyExchangeRepository;
//        private readonly IDistributedCache _cache; // For Redis

//        public CurrencyExchangeService(CurrencyExchangeRepository repo, IDistributedCache cache)
//        {
//            _currencyExchangeRepository = repo;
//            _cache = cache;
//        }

//        public async Task<CurrencyExchangeRate?> GetExchangeRateAsync(CurrencyExchangeRate exchangeRate)
//        {
//            string cacheKey = RedisSettings.GetObjectKey(exchangeRate.Code, exchangeRate.ExchangeDate);
//            var exchangeRateCached = await _cache.GetStringAsync(cacheKey);

//            if (exchangeRateCached != null)
//            {
//                return JsonSerializer.Deserialize<CurrencyExchangeRate>(exchangeRateCached);
//            }

//            var exchangeRateDb = await _currencyExchangeRepository.GetCurrencyExchange(exchangeRate.);
//            if (exchangeRateDb != null)
//            {
//                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(user),
//                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow =RedisSettings.GetCacheTimeSpan });
//            }

//            return user;
//        }

//        public async Task AddUserAsync(CurrencyExchangeRate exchangeRate)
//        {
//            await _currencyExchangeRepository.AddExchangeRate(exchangeRate);
//        }


//    }

//}

