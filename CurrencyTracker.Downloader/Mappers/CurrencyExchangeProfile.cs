using AutoMapper;

using DownloaderModel = CurrencyTracker.Downloader.Models;
using ServiceModel = CurrencyTracker.Services.Models;

namespace CurrencyTracker.Downloader.Mappers
{

    public class CurrencyExchangeProfile : Profile
    {
        public CurrencyExchangeProfile()
        {
            CreateMap<DownloaderModel.CurrencyExchangeRateNBP, ServiceModel.CurrencyExchangeRate>()
                .ForMember(dest => dest.BaseName, opt => opt.Ignore()) // NBP API only has data for PLN so we ignore it for now
                .ForMember(dest => dest.BaseCode, opt => opt.Ignore()) // as above
                .ForMember(dest => dest.TargetName, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.TargetCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Ask, opt => opt.MapFrom(src => (src.Rates != null && src.Rates.Any()) ? src.Rates.First().Ask : 0))
                .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => (src.Rates != null && src.Rates.Any()) ? src.Rates.First().Bid : 0))
                .ForMember(dest => dest.ExchangeDate, opt => opt.MapFrom(src => (src.Rates != null && src.Rates.Any() ? DateOnly.Parse(src.Rates.First().EffectiveDate ?? "") : DateOnly.MinValue)))
                .ReverseMap();
        }
    }
}
