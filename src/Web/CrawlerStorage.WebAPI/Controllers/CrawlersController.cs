namespace CrawlerStorage.WebAPI.Controllers;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Crawlers;
using CrawlerStorage.Data.Repositories;

using Microsoft.AspNetCore.Mvc;

public class CrawlersController : BaseController
{
    private readonly IMapper mapper;
    private readonly CrawlerStorageRepository<Crawler> repository;

    public CrawlersController(IMapper mapper, CrawlerStorageRepository<Crawler> repository)
    {
        this.mapper = mapper;
        this.repository = repository;
    }

    [HttpGet(Name = nameof(GetAll))]
    public async Task<IActionResult> GetAll()
    {
        var crawlers = this.repository.All();
        var crawlerReadDtos = this.mapper.Map<IEnumerable<CrawlerReadDto>>(crawlers);

        return this.Ok(crawlerReadDtos);
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var crawler = await this.repository.FindAsync(id);

        if (crawler != null)
        {
            var crawlerReadDto = this.mapper.Map<CrawlerReadDto>(crawler);

            return this.Ok(crawlerReadDto);
        }

        return this.NotFound();
    }

    [HttpPost(Name = nameof(Create))]
    public async Task<IActionResult> Create(CrawlerCreateDto model)
    {
        var crawler = this.mapper.Map<Crawler>(model);

        await this.repository.AddAsync(crawler);
        await this.repository.SaveChangesAsync();

        var crawlerReadDto = this.mapper.Map<CrawlerReadDto>(crawler);

        return this.CreatedAtRoute(nameof(GetById), new { crawlerReadDto.Id }, crawlerReadDto);
    }
}