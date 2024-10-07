namespace CrawlerStorage.Services.Intrefaces;

using CrawlerStorage.Data.Models.Dtos.Groups;

public interface IGroupsService
{
    Task<GroupDto> CreateAsync(GroupInputDto input);

    void Process(GroupDto groupDto);

    bool CheckForUpdate(GroupDto groupDto, GroupDto dbGroupDto);

    void Update(GroupDto groupDto, GroupDto dbGroupDto);
}