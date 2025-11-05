using AutoMapper;
using CurrencyTracker.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ApiModels = CurrencyTracker.API.Models;

namespace CurrencyTracker.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly ILogger<CurrencyExchangeController> _logger;
        private readonly IMapper _mapper;
        private ICurrencyExchangeService _currencyExchangeService;
        private ICurrencyService _currencyService;
        private HashSet<string> _avaliableCurrencies = new HashSet<string>();

        public CurrencyExchangeController(ILogger<CurrencyExchangeController> logger, IMapper mapper, ICurrencyExchangeService currencyExchangeService, ICurrencyService currencyService)
        {
            _logger = logger;
            _mapper = mapper;
            _currencyExchangeService = currencyExchangeService;
            _currencyService = currencyService;
        }

        [Authorize]
        [HttpGet(Name = "ExchangeRate/")]
        public async Task<ActionResult<ApiModels.CurrencyExchangeRate>> Get(string baseCurrencyCode, [FromQuery] string targetCurrencyCode, [FromQuery] DateOnly date)
        {
            if (string.IsNullOrEmpty(baseCurrencyCode) || string.IsNullOrEmpty(targetCurrencyCode))
            {
                return BadRequest("Code is empty");
            }

            if(_avaliableCurrencies.Count == 0)
            {
                _avaliableCurrencies = await _currencyService.GetCurrenciesAsync() ?? new HashSet<string>();
            }
            
            if (!_avaliableCurrencies.Contains(baseCurrencyCode) || !_avaliableCurrencies.Contains(targetCurrencyCode))
            {
                return BadRequest("Wrong currency code");
            }

            Services.Models.CurrencyExchangeRate? result = await _currencyExchangeService.GetExchangeRateAsync(baseCurrencyCode, targetCurrencyCode, date);

            if (result == null)
            {
                return NotFound();
            }

            ApiModels.CurrencyExchangeRate resultMapped = _mapper.Map<ApiModels.CurrencyExchangeRate>(result);

            return Ok(resultMapped);
        }

        [Authorize]
        [HttpPost(Name = "ExchangeRate")]
        public async Task<ActionResult<ApiModels.CurrencyExchangeRate>> Post([FromBody] ApiModels.CurrencyExchangeRate currencyExchangeRate)
        {
            if (_avaliableCurrencies.Count == 0)
            {
                _avaliableCurrencies = await _currencyService.GetCurrenciesAsync() ?? new HashSet<string>();
            }

            if (!_avaliableCurrencies.Contains(currencyExchangeRate.BaseCode) || !_avaliableCurrencies.Contains(currencyExchangeRate.TargetCode))
            {
                return BadRequest("Wrong currency code");
            }

            var exchangeRateMapped = _mapper.Map<ApiModels.CurrencyExchangeRate, Services.Models.CurrencyExchangeRate>(currencyExchangeRate);

            bool result = await _currencyExchangeService.AddExchangeRateAsync(exchangeRateMapped);

            if (!result)
            {
                return NotFound("Error while adding exchange rate");
            }

            return Ok();
        }
    }
}
