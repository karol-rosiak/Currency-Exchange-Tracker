
using CurrencyTracker.Services.Models;

namespace CurrencyTracker.Services.Services
{
    public interface ICurrencyExchangeService
    {
        public Task<CurrencyExchangeRate?> GetExchangeRateAsync(string baseCode, string targetCode, DateOnly exchangeDate);

        public Task<bool> AddExchangeRateAsync(CurrencyExchangeRate exchangeRate);
    }

}

