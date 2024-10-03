namespace CrawlerStorage.Services.Intrefaces;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Groups;

public interface IGroupsService
{
    Task<Group> CreateAsync(GroupInputDto input);

    Task ProcessAsync(Group group);
}