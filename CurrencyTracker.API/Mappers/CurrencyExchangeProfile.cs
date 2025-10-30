using AutoMapper;

using ApiModel = CurrencyTracker.API.Models;
using ServiceModel = CurrencyTracker.Services.Models;

namespace CurrencyTracker.API.Mappers
{

    public class CurrencyExchangeProfile : Profile
    {
        public CurrencyExchangeProfile()
        {
            CreateMap<ServiceModel.CurrencyExchangeRate,ApiModel.CurrencyExchangeRate>().ReverseMap();
        }
    }
}
