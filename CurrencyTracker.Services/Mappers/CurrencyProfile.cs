using AutoMapper;

using DataEntity = CurrencyTracker.Data.Entities;
using ServiceModel = CurrencyTracker.Services.Models;

namespace CurrencyTracker.Services.Mappers
{

    public class CurrencyProfile : Profile
    {
        public CurrencyProfile()
        {
            CreateMap<DataEntity.Currency, ServiceModel.Currency>().ReverseMap();
        }
    }
}
