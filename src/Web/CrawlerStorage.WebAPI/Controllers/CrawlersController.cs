namespace CrawlerStorage.WebAPI.Controllers;

using AutoMapper;

using CrawlerStorage.Data;
using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos;

using Microsoft.AspNetCore.Mvc;

public class CrawlersController : BaseController
{
    private readonly IMapper mapper;
    private readonly CrawlerStorageDbContext context;

    public CrawlersController(IMapper mapper, CrawlerStorageDbContext context)
    {
        this.mapper = mapper;
        this.context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var crawlerReadDtos = this.mapper.Map<IEnumerable<CrawlerReadDto>>(this.context.Crawlers);
        return this.Ok(crawlerReadDtos);
    }

    [HttpGet("{id}", Name = "GetById")]
    public IActionResult GetById(int id)
    {
        return this.Ok(id);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrawlerCreateDto model)
    {
        var crawler = this.mapper.Map<Crawler>(model);

        this.context.Crawlers.Add(crawler);
        this.context.SaveChanges();

        return this.CreatedAtRoute(nameof(GetById), new { crawler.Id }, crawler);
    }
}