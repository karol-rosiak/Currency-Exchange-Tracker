using AutoMapper;
using CurrencyTracker.Data.Cache;
using CurrencyTracker.DataDatabase.Repositories;
using CurrencyTracker.Services.Models;
using CurrencyTracker.Services.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using DataEntity = CurrencyTracker.Data.Entities;
using ServiceModel = CurrencyTracker.Services.Models;


namespace CurrencyTracker.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache; 

        public CurrencyService(ICurrencyRepository repo, IMapper mapper, IDistributedCache cache)
        {
            _currencyRepository = repo;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<HashSet<string>?> GetCurrenciesAsync()
        {
            string cacheKey = RedisSettings.GetCurrenciesKey();
            string? currenciesCached = await _cache.GetStringAsync(cacheKey);

            if (currenciesCached != null)
            {
                return JsonSerializer.Deserialize<HashSet<string>>(currenciesCached);
            }

            List<DataEntity.Currency> currenciesDb = await _currencyRepository.GetCurrencies();
            HashSet<string>? currencySet = currenciesDb.Select(c => c.Code).ToHashSet();

            if (currencySet.Count > 0)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(currencySet),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = RedisSettings.GetCacheTimeSpan });
            }

            return currencySet;
        }

        public async Task<Currency?> GetCurrencyAsync(string code)
        {
            string cacheKey = RedisSettings.GetCurrencyKey(code);
            string? currencyCached = await _cache.GetStringAsync(cacheKey);

            if (currencyCached != null)
            {
                return JsonSerializer.Deserialize<Currency>(currencyCached);
            }

            DataEntity.Currency? currencyDb = await _currencyRepository.GetCurrencyByCode(code);

            if (currencyDb != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(currencyDb),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = RedisSettings.GetCacheTimeSpan });
            }

            return _mapper.Map<Currency>(currencyDb);
        }
    }

}

