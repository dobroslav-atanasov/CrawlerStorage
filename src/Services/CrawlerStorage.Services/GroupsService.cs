namespace CrawlerStorage.Services;

using System;
using System.Threading.Tasks;

using CrawlerStorage.Common.Helpers;
using CrawlerStorage.Data.Models.DbEntities;
using CrawlerStorage.Data.Models.Dtos.Groups;
using CrawlerStorage.Data.Models.Enumerations;
using CrawlerStorage.Services.Intrefaces;

public class GroupsService : IGroupsService
{
    private readonly HttpClient httpClient;

    public GroupsService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task ProcessAsync(Group group)
    {
        foreach (var document in group.Documents)
        {
            document.Identifier = Guid.NewGuid();
            document.MD5 = MD5Helper.Hash(document.Content);
            document.OperationId = (int)OperationType.Add;
        }

        var folder = group.Name;
        var zipFolderName = $"{folder}.zip";
        group.Name = zipFolderName;
        group.Identifier = Guid.NewGuid();
        group.OperationId = (int)OperationType.Add;
        group.Content = ZipHelper.ZipGroup(group);

        //var dbGroup = await this.GetGroupAsync(group.CrawlerId, group.Name);

        //if (dbGroup == null)
        //{
        //    await this.AddGroupAsync(group);
        //    var log = new Log
        //    {
        //        Identifier = group.Identifier,
        //        LogDate = DateTime.UtcNow,
        //        Operation = (int)OperationType.Add,
        //        CrawlerId = group.CrawlerId,
        //    };
        //    await this.logsService.AddLogAsync(log);
        //}
        //else
        //{
        //    var isUpdated = this.CheckForUpdate(group, dbGroup);
        //    if (isUpdated)
        //    {
        //        await this.UpdateGroupAsync(group, dbGroup);
        //        await this.logsService.UpdateLogAsync(dbGroup.Identifier, (int)OperationType.Update);
        //    }
        //    else
        //    {
        //        await this.logsService.UpdateLogAsync(dbGroup.Identifier, (int)OperationType.None);
        //    }
        //}
    }

    public async Task<Group> CreateAsync(GroupInputDto input)
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

            return new Group
            {
                Name = name,
                CrawlerId = input.CrawlerId,
                Documents = [documentDto]
            };
        }

        return null;
    }

    private async Task<Document> CreateDocumentAsync(HttpResponseMessage response, string url)
    {
        var name = this.CreateNameFromUrl(url);
        var document = new Document
        {
            Name = name.ToLower(),
            Format = response.Content.Headers.ContentType?.MediaType,
            Url = url,
            Content = await response.Content.ReadAsByteArrayAsync(),
            Encoding = (response.Content.Headers.ContentType?.CharSet) ?? "utf-8",
            Order = 1
        };

        return document;
    }

    private string CreateNameFromUrl(string url)
    {
        var uri = new Uri(url);
        return $"{uri.Host}_{string.Join("_", uri.Segments.Where(x => x != "/").Select(x => x.Replace("/", string.Empty)))}".ToLower();
    }
}