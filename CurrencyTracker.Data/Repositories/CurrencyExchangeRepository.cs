using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CurrencyTracker.Database.Data;
using CurrencyTracker.Database.Models;
using CurrencyTracker.Enums;
using CurrencyTracker.Helpers;
using CurrencyTracker.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CurrencyTracker.Database.Repositories
{
    public class CurrencyExchangeRepository
    {
        private readonly CurrencyDbContext _context;

        public CurrencyExchangeRepository(CurrencyDbContext context)
        {
            _context = context;
        }

        public async Task<API.Models.CurrencyExchangeRate?> GetCurrencyExchange(CurrencyEnum currency, DateOnly date)
        {
            API.Database.Models.CurrencyRate? exchangeRate = await _context.CurrencyRates.Where(c => c.Currency.Value == (int)currency && c.ExchangeDate == date).FirstOrDefaultAsync();
            if (exchangeRate == null)
            {
                return null;
            }

            // change to a mapper 
            return new API.Models.CurrencyExchangeRate
            {
                Ask = exchangeRate.Ask,
                Bid = exchangeRate.Bid,
                Code = exchangeRate.Currency.Code,
                Currency = exchangeRate.Currency.Name,
                ExchangeDate = exchangeRate.ExchangeDate
            };
        }

        // Add user
        public async Task AddExchangeRate(API.Models.CurrencyExchangeRate exchangeRate)
        {
            Database.Models.CurrencyRate exchangeRateDB = new()
            {
                Ask = exchangeRate.Ask ?? 0,
                Bid = exchangeRate.Bid ?? 0,
                ExchangeDate = exchangeRate.ExchangeDate ?? DateOnly.MinValue,
                CreateDate = DateTime.Now,
            };

            _context.CurrencyRates.Add(exchangeRateDB);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(API.Models.CurrencyExchangeRate exchangeRate)
        {
            Database.Models.CurrencyRate exchangeRateDB = new()
            {
                Ask = exchangeRate.Ask ?? 0,
                Bid = exchangeRate.Bid ?? 0,
                ExchangeDate = exchangeRate.ExchangeDate ?? DateOnly.MinValue,
                CreateDate = DateTime.Now,
            };

            _context.CurrencyRates.Update(exchangeRateDB);
            await _context.SaveChangesAsync();
        }
    }
}
