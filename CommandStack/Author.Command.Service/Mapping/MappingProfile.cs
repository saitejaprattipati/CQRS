using Author.Command.Domain.Command;
using Author.Command.Domain.Models;
using Author.Command.Persistence.DBContextAggregate;
using AutoMapper;

namespace Author.Command.Service.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSystemUserCommand, SystemUsers>();
            CreateMap<UpdateSystemUserCommand, SystemUsers>();
            CreateMap<CreateContentDisclaimerCommand, Disclaimers>()
                    .ForMember(dest => dest.Name, opts => opts.MapFrom(m => m.GroupName))
                    .ForMember(dest => dest.DefaultCountryId, opts => opts.MapFrom(m => m.DefaultCountryId))
                    .ForMember(dest => dest.DisclaimerContents, opts => opts.MapFrom(m => m.DisclaimerContent));
            CreateMap<DisclaimerContent, DisclaimerContents>();
        }
    }
}
