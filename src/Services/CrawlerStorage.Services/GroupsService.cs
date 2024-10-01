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
                case 1:
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    while (responseAsString == "Rate Limit Exceeded")
                    {
                        await Task.Delay(3000);
                        response = await this.httpClient.GetAsync(input.Url);
                        responseAsString = await response.Content.ReadAsStringAsync();
                    }
                    break;
            }

            var name = this.CreateGroupName(input.Url);
            var documents = this.CreateDocument(response);

            return new GroupCreateDto
            {
                Name = name,
                CrawlerId = input.CrawlerId,
                Documents = documents
            };
        }

        return null;
    }

    private List<DocumentDto> CreateDocument(HttpResponseMessage response)
    {
        throw new NotImplementedException();
    }

    private string CreateGroupName(string url)
    {
        var uri = new Uri(url);
        return $"{uri.Host}_{string.Join("_", uri.Segments.Where(x => x != "/").Select(x => x.Replace("/", string.Empty)))}";
    }
}