namespace CrawlerStorage.WebAPI.Controllers;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Documents;
using CrawlerStorage.Data.Repositories;

using Microsoft.AspNetCore.Mvc;

public class DocumentsController : BaseController
{
    private readonly IMapper mapper;
    private readonly CrawlerStorageRepository<Group> repository;

    public DocumentsController(IMapper mapper, CrawlerStorageRepository<Group> repository)
    {
        this.mapper = mapper;
        this.repository = repository;
    }

    [HttpGet("{id}", Name = nameof(GetDocumentsByGroupId))]
    public async Task<IActionResult> GetDocumentsByGroupId(int id)
    {
        var group = await this.repository.FindAsync(id);

        if (group != null)
        {
            var documentReadDtos = this.mapper.Map<IEnumerable<DocumentReadDto>>(group.Documents);
            return this.Ok(documentReadDtos);
        }

        return this.NotFound();
    }
}