using AutoMapper;
using BalisStandard;

namespace BalisWpf
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<KomBankRatesLine, KomBankRateVm>();
        }
    }
}
