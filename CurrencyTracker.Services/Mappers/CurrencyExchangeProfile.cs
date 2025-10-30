using AutoMapper;

using DataEntity = CurrencyTracker.Data.Entities;
using ServiceModel = CurrencyTracker.Services.Models;

namespace CurrencyTracker.Services.Mappers
{

    public class CurrencyExchangeProfile : Profile
    {
        public CurrencyExchangeProfile()
        {
            CreateMap<DataEntity.CurrencyExchangeRate, ServiceModel.CurrencyExchangeRate>()
                .ForMember(dest => dest.BaseName, opt => opt.MapFrom(src => src.BaseCurrency.Name))
                .ForMember(dest => dest.BaseCode, opt => opt.MapFrom(src => src.BaseCurrency.Code))
                .ForMember(dest => dest.TargetName, opt => opt.MapFrom(src => src.TargetCurrency.Name))
                .ForMember(dest => dest.TargetCode, opt => opt.MapFrom(src => src.TargetCurrency.Code))
                .ReverseMap();
        }
    }
}
