namespace CrawlerStorage.Services.Automapper.Profiles;

using AutoMapper;

using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Documents;

public class DocumentsProfile : Profile
{
    public DocumentsProfile()
    {
        this.CreateMap<DocumentDto, Document>()
            .ReverseMap();
    }
}