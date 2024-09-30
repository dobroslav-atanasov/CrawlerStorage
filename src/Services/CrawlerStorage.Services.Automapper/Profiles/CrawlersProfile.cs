namespace CrawlerStorage.Services.Automapper.Profiles;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos;

public class CrawlersProfile : Profile
{
    public CrawlersProfile()
    {
        this.CreateMap<CrawlerCreateDto, Crawler>();

        this.CreateMap<Crawler, CrawlerReadDto>();
    }
}