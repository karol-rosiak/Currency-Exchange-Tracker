//using Microsoft.AspNetCore.Mvc;
//using CurrencyTracker.Enums;
//using CurrencyTracker.Models.API;

//namespace NBPCurrencyTracker.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class CurrencyExchangeController : ControllerBase
//    {
//        private readonly ILogger<CurrencyExchangeController> _logger;

//        public CurrencyExchangeController(ILogger<CurrencyExchangeController> logger)
//        {
//            _logger = logger;
//        }

//        [HttpGet(Name = "ExchangeRate")]
//        public CurrencyExchangeRate Get([FromQuery] CurrencyEnum currency, [FromQuery] DateTime date)
//        {
//            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
//            {
//                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                TemperatureC = Random.Shared.Next(-20, 55),
//                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
//            })
//            .ToArray();
//        }
//    }
//}
