using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Data.Context;
using CurrencyTracker.Data.Entities;


namespace CurrencyTracker.DataDatabase.Repositories
{
    public class CurrencyExchangeRepository : ICurrencyExchangeRepository
    {
        private readonly CurrencyDbContext _context;

        public CurrencyExchangeRepository(CurrencyDbContext context)
        {
            _context = context;
        }

        public async Task<CurrencyExchangeRate?> GetCurrencyExchange(string baseCurrencyCode, string targetCurrencyCode, DateOnly date)
        {
            return await _context.CurrencyExchangeRates
                .Where(c => c.BaseCurrency.Code == baseCurrencyCode && c.ExchangeDate == date)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddExchangeRate(CurrencyExchangeRate exchangeRate)
        {
            try
            {
                _context.CurrencyExchangeRates.Add(exchangeRate);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // TODO: add error logging here
                return false;
            }

            return true;
        }

    }
}
