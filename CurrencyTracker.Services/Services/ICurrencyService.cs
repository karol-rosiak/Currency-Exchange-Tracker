
using CurrencyTracker.Services.Models;

namespace CurrencyTracker.Services.Services
{
    public interface ICurrencyService
    {
        public Task<HashSet<string>?> GetCurrenciesAsync();
        public Task<Currency?> GetCurrencyAsync(string code);
    }

}

