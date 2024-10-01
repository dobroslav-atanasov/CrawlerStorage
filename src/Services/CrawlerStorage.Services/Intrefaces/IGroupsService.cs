namespace CrawlerStorage.Services.Intrefaces;

using CrawlerStorage.Data.Models.Dtos.Groups;

public interface IGroupsService
{
    Task<GroupCreateDto> ProcessAsync(GroupInputDto input);
}