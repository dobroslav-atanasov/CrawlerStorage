namespace CrawlerStorage.WebAPI.Controllers;

using AutoMapper;

using CrawlerStorage.Common.Helpers;
using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Documents;
using CrawlerStorage.Data.Models.Dtos.Groups;
using CrawlerStorage.Data.Models.Enumerations;
using CrawlerStorage.Data.Repositories;
using CrawlerStorage.Services.Intrefaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GroupsController : BaseController
{
    private readonly IMapper mapper;
    private readonly CrawlerStorageRepository<Group> repository;
    private readonly IHttpService httpService;
    private readonly IGroupsService groupsService;
    private readonly IDocumentsService documentsService;

    public GroupsController(IMapper mapper, CrawlerStorageRepository<Group> repository, IHttpService httpService, IGroupsService groupsService, IDocumentsService documentsService)
    {
        this.mapper = mapper;
        this.repository = repository;
        this.httpService = httpService;
        this.groupsService = groupsService;
        this.documentsService = documentsService;
    }

    [HttpGet(Name = nameof(GetAllGroups))]
    public IActionResult GetAllGroups()
    {
        var groups = this.repository.AllAsNoTracking();
        var groupReadDtos = this.mapper.Map<IEnumerable<GroupReadDto>>(groups);

        return this.Ok(groupReadDtos);
    }

    [HttpGet("{id}", Name = nameof(GetGroupById))]
    public async Task<IActionResult> GetGroupById(int id)
    {
        var group = await this.repository.FindAsync(id);
        if (group != null)
        {
            var groupReadDto = this.mapper.Map<GroupReadDto>(group);
            return this.Ok(groupReadDto);
        }

        return this.NotFound();
    }

    [HttpPost(Name = nameof(CreateGroup))]
    public async Task<IActionResult> CreateGroup(GroupInputDto model)
    {
        var httpModel = await this.httpService.GetAsync(model.Url, model.CrawlerId);

        var documentCreateDtos = new List<DocumentCreateDto>();
        if (httpModel != null)
        {
            var documentCreateDto = this.documentsService.Create(httpModel);
            documentCreateDtos.Add(documentCreateDto);

            if (model.DocumentUrls.Count > 0)
            {
                foreach (var url in model.DocumentUrls)
                {
                    var documentHttpModel = await this.httpService.GetAsync(url, model.CrawlerId);
                    if (documentHttpModel != null)
                    {
                        documentCreateDtos.Add(this.documentsService.Create(documentHttpModel));
                    }
                }
            }

            var groupCreateDto = new GroupCreateDto
            {
                CrawlerId = model.CrawlerId,
                Documents = documentCreateDtos,
                Name = $"{httpModel.Name}.zip",
                Identifier = Guid.NewGuid(),
                Operation = OperationType.Add.ToString(),
            };

            groupCreateDto.Content = ZipHelper.ZipGroup(groupCreateDto);

        }



        var groupDto = await this.groupsService.CreateAsync(model);

        if (groupDto != null)
        {
            this.groupsService.Process(groupDto);

            var dbGroup = await this.repository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == groupDto.Name && x.CrawlerId == model.CrawlerId);

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
                var dbGroupDto = this.mapper.Map<GroupDto>(dbGroup);
                var isUpdated = this.groupsService.CheckForUpdate(groupDto, dbGroupDto);

                if (isUpdated)
                {
                    this.groupsService.Update(groupDto, dbGroupDto);
                    var group = this.mapper.Map<Group>(dbGroupDto);

                    this.repository.Update(group);
                    await this.repository.SaveChangesAsync();
                }

                return this.NoContent();
            }
        }

        return this.BadRequest();
    }
}