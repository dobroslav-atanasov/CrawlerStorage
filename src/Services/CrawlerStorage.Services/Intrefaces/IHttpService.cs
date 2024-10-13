namespace CrawlerStorage.Services.Intrefaces;

using CrawlerStorage.Data.Models.Http;

public interface IHttpService
{
    Task<HttpModel> GetAsync(string url, int crawlerId);
}