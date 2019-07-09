using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Author.Command.Service.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<CreateSystemUserCommand, SystemUsers>()
                .ForMember(dest => dest.FirstName, source => source.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, source => source.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Level, source => source.MapFrom(src => src.Level))
                .ForMember(dest => dest.WorkPhoneNumber, source => source.MapFrom(src => src.WorkPhoneNumber))
                .ForMember(dest => dest.MobilePhoneNumber, source => source.MapFrom(src => src.MobilePhoneNumber))
                .ForMember(dest => dest.Location, source => source.MapFrom(src => src.Location))
                .ForMember(dest => dest.Role, source => source.MapFrom(src => Convert.ToInt32(src.Role)));

            CreateMap<UpdateSystemUserCommand, SystemUsers>()
                .ForMember(dest => dest.FirstName, source => source.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, source => source.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Level, source => source.MapFrom(src => src.Level))
                .ForMember(dest => dest.WorkPhoneNumber, source => source.MapFrom(src => src.WorkPhoneNumber))
                .ForMember(dest => dest.MobilePhoneNumber, source => source.MapFrom(src => src.MobilePhoneNumber))
                .ForMember(dest => dest.Location, source => source.MapFrom(src => src.Location))
                .ForMember(dest => dest.Role, source => source.MapFrom(src => Convert.ToInt32(src.Role)));
        }
    }
}
