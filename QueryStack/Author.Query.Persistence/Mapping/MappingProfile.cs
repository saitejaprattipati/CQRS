using Author.Core.Framework;
using Author.Query.Domain.DBAggregate;
using Author.Query.Persistence.DTO;
using AutoMapper;
using System.Collections.Generic;

namespace Author.Query.Persistence.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Languages, LanguageDTO>();
            //CreateMap<List<Languages>, List<LanguageDTO>>();
            CreateMap<Countries, CountryDTO>()
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.DisplayNameShort, opt => opt.MapFrom(src => Helper.ReplaceChars(src.DisplayName)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => Helper.ReplaceChars(src.DisplayName)))
                .ForMember(dest => dest.Path, opt => opt.MapFrom(src => Helper.ReplaceChars(src.DisplayName)));
            CreateMap<Images, ImageDTO>();
            CreateMap<Disclaimers, DisclaimerDTO>();
            CreateMap<List<Images>, List<ImageDTO>>();
            CreateMap<ResourceGroups,ResourceGroupDTO>();
            CreateMap<Articles, ArticleDTO>();
            CreateMap<Contacts, ContactDTO>();
            CreateMap<Address, AddressDTO>();
            CreateMap<RelatedTaxTagsSchema, TaxTagsDTO>();
            CreateMap<ResourceGroupsSchema, ResourceGroupDTO>();
            CreateMap<ProvinceSchema, ProvinceDTO>();
        }
    }
}
