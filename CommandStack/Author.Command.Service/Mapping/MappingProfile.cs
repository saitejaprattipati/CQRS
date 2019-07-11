using Author.Command.Domain.Command;
using Author.Command.Persistence.DBContextAggregate;
using AutoMapper;

namespace Author.Command.Service.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSystemUserCommand, SystemUsers>();
            CreateMap<UpdateSystemUserCommand, SystemUsers>();
        }
    }
}
