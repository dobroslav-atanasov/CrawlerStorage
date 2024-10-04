namespace CrawlerStorage.Services.Intrefaces;

using CrawlerStorage.Data.Models.Dtos.Groups;

public interface IGroupsService
{
    Task<GroupDto> CreateAsync(GroupInputDto input);

    Task ProcessAsync(GroupDto group);
}