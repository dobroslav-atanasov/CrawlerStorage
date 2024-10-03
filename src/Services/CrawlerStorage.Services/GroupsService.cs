namespace CrawlerStorage.Services;

using System;
using System.Threading.Tasks;

using CrawlerStorage.Data.Models.Dtos.Documents;
using CrawlerStorage.Data.Models.Dtos.Groups;
using CrawlerStorage.Services.Intrefaces;

public class GroupsService : IGroupsService
{
    private readonly HttpClient httpClient;

    public GroupsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<GroupCreateDto> ProcessAsync(GroupInputDto input)
    {
        var response = await this.httpClient.GetAsync(input.Url);

        if (response.IsSuccessStatusCode)
        {
            switch (input.CrawlerId)
            {
                case 2:
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    while (responseAsString == "Rate Limit Exceeded")
                    {
                        await Task.Delay(3000);
                        response = await this.httpClient.GetAsync(input.Url);
                        responseAsString = await response.Content.ReadAsStringAsync();
                    }
                    break;
            }

            var name = this.CreateNameFromUrl(input.Url);
            var documentDto = await this.CreateDocumentAsync(response, input.Url);

            return new GroupCreateDto
            {
                Name = name,
                CrawlerId = input.CrawlerId,
                Documents = [documentDto]
            };
        }

        return null;
    }

    private async Task<DocumentDto> CreateDocumentAsync(HttpResponseMessage response, string url)
    {
        var name = this.CreateNameFromUrl(url);
        var documentDto = new DocumentDto
        {
            Name = name.ToLower(),
            Format = response.Content.Headers.ContentType?.MediaType,
            Url = url,
            Content = await response.Content.ReadAsByteArrayAsync(),
            Encoding = (response.Content.Headers.ContentType?.CharSet) ?? "utf-8",
            Order = 1
        };

        return documentDto;
    }

    private string CreateNameFromUrl(string url)
    {
        var uri = new Uri(url);
        return $"{uri.Host}_{string.Join("_", uri.Segments.Where(x => x != "/").Select(x => x.Replace("/", string.Empty)))}";
    }
}