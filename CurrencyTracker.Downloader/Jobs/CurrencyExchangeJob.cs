using AutoMapper;
using CurrencyTracker.Downloader.Jobs.Settings;
using CurrencyTracker.Services.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Text.Json;

using DownloaderModels = CurrencyTracker.Downloader.Models;
using ServicesModels = CurrencyTracker.Services.Models;
namespace CurrencyTracker.Downloader.Jobs
{
    public class CurrencyExchangeJob : IJob
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICurrencyService _currencyService;
        private readonly ICurrencyExchangeService _currencyExchangeService;
        private readonly IMapper _mapper;
        private readonly ILogger<CurrencyExchangeJob> _logger;
        private readonly CurrencyExchangeJobSettings _settings;

        public CurrencyExchangeJob(IHttpClientFactory httpClientFactory, ICurrencyService currencyService, ICurrencyExchangeService currencyExchangeService, IMapper mapper, ILogger<CurrencyExchangeJob> logger, IOptions<CurrencyExchangeJobSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _currencyExchangeService = currencyExchangeService;
            _currencyService = currencyService;
            _mapper = mapper;
            _logger = logger;
            _settings = settings.Value;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("CurrencyJob started.");

            HashSet<string>? currencies = await _currencyService.GetCurrenciesAsync();

            currencies?.Remove("PLN");

            if (currencies == null)
            {
                _logger.LogWarning("No currencies found in the database.");
                return;
            }

            foreach (var code in currencies)
            {
                await ProcessCurrencyAsync(code, context.CancellationToken);
            }

            _logger.LogInformation("CurrencyJob finished.");
        }

        private async Task ProcessCurrencyAsync(string currencyCode, CancellationToken cancellationToken)
        {
            int attempt = 0;
            bool success = false;

            while (!success && attempt < _settings.MaxRetries)
            {
                try
                {
                    attempt++;
                    HttpClient client = _httpClientFactory.CreateClient();
                    string url = $"https://api.nbp.pl/api/exchangerates/rates/c/{currencyCode.ToLower()}/today/?format=json"; // TODO: Move url so it's not harcoded
                    string response = await client.GetStringAsync(url);

                    DownloaderModels.CurrencyExchangeRateNBP? currencyExchangeRate = JsonSerializer.Deserialize<DownloaderModels.CurrencyExchangeRateNBP>(response);

                    if (currencyExchangeRate?.Rates?.Length > 0)
                    {
                        ServicesModels.CurrencyExchangeRate currencyExchangeRateMapped = _mapper.Map<ServicesModels.CurrencyExchangeRate>(currencyExchangeRate);
                        currencyExchangeRateMapped.BaseCode = "PLN"; // TODO: Find a better way so it's not hard coded
                        await _currencyExchangeService.AddExchangeRateAsync(currencyExchangeRateMapped);
                        _logger.LogInformation($"[{currencyCode}] Saved rate for {currencyExchangeRate.Rates[0].EffectiveDate}");
                        success = true;
                    }
                    else
                    {
                        throw new Exception("No rates returned.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning($"[{currencyCode}] Attempt {attempt}: API request failed ({ex.Message})");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"[{currencyCode}] Attempt {attempt} failed: {ex.Message}");
                }

                if (!success && attempt < _settings.MaxRetries)
                {
                    await Task.Delay(TimeSpan.FromHours(_settings.RetryDelayHours * attempt), cancellationToken);
                }
            }

            if (!success)
            {
                _logger.LogError($"[{currencyCode}] Failed after {_settings.MaxRetries} retries.");
            }

        }
    }
}
