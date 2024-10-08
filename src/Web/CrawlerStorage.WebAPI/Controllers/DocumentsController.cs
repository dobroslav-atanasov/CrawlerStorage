namespace CrawlerStorage.WebAPI.Controllers;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Documents;
using CrawlerStorage.Data.Repositories;

using Microsoft.AspNetCore.Mvc;

[Route("api/v{version:apiVersion}/Groups/{groupId}/[controller]")]
public class DocumentsController : BaseController
{
    private readonly IMapper mapper;
    private readonly CrawlerStorageRepository<Group> repository;

    public DocumentsController(IMapper mapper, CrawlerStorageRepository<Group> repository)
    {
        this.mapper = mapper;
        this.repository = repository;
    }

    [HttpGet(Name = nameof(GetDocumentsByGroupId))]
    public async Task<IActionResult> GetDocumentsByGroupId(int groupId)
    {
        var group = await this.repository.FindAsync(groupId);

        if (group != null)
        {
            var documentReadDtos = this.mapper.Map<IEnumerable<DocumentReadDto>>(group.Documents);
            return this.Ok(documentReadDtos);
        }

        return this.NotFound();
    }

    [HttpGet("{id}", Name = nameof(GetDocumentByGroupId))]
    public async Task<IActionResult> GetDocumentByGroupId(int groupId, int id)
    {
        var group = await this.repository.FindAsync(groupId);

        if (group != null)
        {
            var document = group.Documents.FirstOrDefault(x => x.Id == id);
            if (document != null)
            {
                var documentReadDto = this.mapper.Map<DocumentReadDto>(document);

                return this.Ok(documentReadDto);
            }
        }

        return this.NotFound();
    }
}