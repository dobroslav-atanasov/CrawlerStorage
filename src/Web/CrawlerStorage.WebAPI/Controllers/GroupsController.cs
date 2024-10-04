namespace CrawlerStorage.WebAPI.Controllers;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Groups;
using CrawlerStorage.Data.Repositories;
using CrawlerStorage.Services.Intrefaces;

using Microsoft.AspNetCore.Mvc;

public class GroupsController : BaseController
{
    private readonly IMapper mapper;
    private readonly CrawlerStorageRepository<Group> repository;
    private readonly IGroupsService groupsService;

    public GroupsController(IMapper mapper, CrawlerStorageRepository<Group> repository, IGroupsService groupsService)
    {
        this.mapper = mapper;
        this.repository = repository;
        this.groupsService = groupsService;
    }

    [HttpGet(Name = nameof(GetAllGroups))]
    public async Task<IActionResult> GetAllGroups()
    {
        return this.Ok();
    }

    [HttpGet("{id}", Name = nameof(GetGroupById))]
    public async Task<IActionResult> GetGroupById(int id)
    {
        return this.Ok();
    }

    [HttpPost(Name = nameof(CreateGroup))]
    public async Task<IActionResult> CreateGroup(GroupInputDto model)
    {
        var groupDto = await this.groupsService.CreateAsync(model);

        if (groupDto != null)
        {
            await this.groupsService.ProcessAsync(groupDto);

            var dbGroup = await this.repository.GetAsync(x => x.Name == groupDto.Name && x.CrawlerId == model.CrawlerId);
            if (dbGroup == null)
            {
                var group = this.mapper.Map<Group>(groupDto);
                await this.repository.AddAsync(group);
                await this.repository.SaveChangesAsync();

                var groupReadDto = this.mapper.Map<GroupReadDto>(group);

                return this.CreatedAtRoute(nameof(GetGroupById), new { group.Id }, groupReadDto);
            }
            else
            {
                //TODO update
            }
        }

        return this.BadRequest();
    }
}