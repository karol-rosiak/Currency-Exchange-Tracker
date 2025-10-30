using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Data.Context;
using CurrencyTracker.Data.Entities;


namespace CurrencyTracker.DataDatabase.Repositories
{
    public interface ICurrencyExchangeRepository
    {

        public Task<CurrencyExchangeRate?> GetCurrencyExchange(string baseCurrencyCode, string targetCurrencyCode, DateOnly date);

        public Task<bool> AddExchangeRate(CurrencyExchangeRate exchangeRate);

    }
}
