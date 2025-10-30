using Microsoft.EntityFrameworkCore;
using CurrencyTracker.Data.Context;
using CurrencyTracker.Data.Entities;


namespace CurrencyTracker.DataDatabase.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyDbContext _context;

        public CurrencyRepository(CurrencyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Currency>> GetCurrencies() => await _context.Currencies.ToListAsync();

        public async Task<Currency?> GetCurrencyByCode(string code) => await _context.Currencies.Where(c => c.Code.ToLower() == code.ToLower()).FirstOrDefaultAsync();
    }
}
