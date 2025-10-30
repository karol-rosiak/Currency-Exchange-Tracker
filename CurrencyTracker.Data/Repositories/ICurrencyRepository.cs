using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Data.Context;
using CurrencyTracker.Data.Entities;


namespace CurrencyTracker.DataDatabase.Repositories
{
    public interface ICurrencyRepository
    {
        public Task<List<Currency>> GetCurrencies();

        public Task<Currency?> GetCurrencyByCode(string code);
    }
}
