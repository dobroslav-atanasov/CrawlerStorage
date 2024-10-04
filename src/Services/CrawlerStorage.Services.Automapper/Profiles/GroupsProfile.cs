namespace CrawlerStorage.Services.Automapper.Profiles;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Groups;

public class GroupsProfile : Profile
{
    public GroupsProfile()
    {
        this.CreateMap<GroupDto, Group>()
            .ForMember(x => x.Documents, opt => opt.MapFrom(src => src.Documents))
            .ReverseMap();

        this.CreateMap<Group, GroupReadDto>();
    }
}