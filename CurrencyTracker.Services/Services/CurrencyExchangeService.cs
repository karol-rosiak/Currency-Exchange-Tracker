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
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ICurrencyExchangeRepository _currencyExchangeRepository;
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache; 

        public CurrencyExchangeService(ICurrencyExchangeRepository currencyExchangeRepository, ICurrencyRepository currencyRepository, ICurrencyService currencyService, IMapper mapper, IDistributedCache cache)
        {
            _currencyExchangeRepository = currencyExchangeRepository;
            _currencyService = currencyService;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ServiceModel.CurrencyExchangeRate?> GetExchangeRateAsync(string baseCode, string targetCode, DateOnly exchangeDate)
        {
            string cacheKey = RedisSettings.GetExchangeRateKey(baseCode, targetCode, exchangeDate);
            var exchangeRateCached = await _cache.GetStringAsync(cacheKey);

            if (exchangeRateCached != null)
            {
                return JsonSerializer.Deserialize<CurrencyExchangeRate>(exchangeRateCached);
            }

            var exchangeRateDb = await _currencyExchangeRepository.GetCurrencyExchange(baseCode,targetCode, exchangeDate);

            if (exchangeRateDb != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(exchangeRateDb),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = RedisSettings.GetCacheTimeSpan });
            }

            return _mapper.Map<CurrencyExchangeRate>(exchangeRateDb); 
        }

        public async Task<bool> AddExchangeRateAsync(CurrencyExchangeRate exchangeRate)
        {
            Currency? baseCurrency = await _currencyService.GetCurrencyAsync(exchangeRate.BaseCode);
            Currency? targetCurrency = await _currencyService.GetCurrencyAsync(exchangeRate.TargetCode);

            if(baseCurrency == null || targetCurrency == null)
            {
                // TODO: add logging here
                return false;
            }

            DataEntity.CurrencyExchangeRate exchangeRateDb = new DataEntity.CurrencyExchangeRate()
            {
                BaseCurrencyId = baseCurrency.Id,
                TargetCurrencyId = targetCurrency.Id,
                Ask = exchangeRate.Ask,
                Bid = exchangeRate.Bid,
                ExchangeDate = exchangeRate.ExchangeDate,
                CreateDate = DateTime.Now
            };

            bool result = await _currencyExchangeRepository.AddExchangeRate(exchangeRateDb);

            return result;
        }


    }

}

