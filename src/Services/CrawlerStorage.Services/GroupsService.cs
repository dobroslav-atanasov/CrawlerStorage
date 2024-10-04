namespace CrawlerStorage.Services;

using System;
using System.Threading.Tasks;

using CrawlerStorage.Common.Helpers;
using CrawlerStorage.Data.Models.Dtos.Documents;
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

    public async Task ProcessAsync(GroupDto groupDto)
    {
        foreach (var documentDto in groupDto.Documents)
        {
            documentDto.Identifier = Guid.NewGuid();
            documentDto.MD5 = MD5Helper.Hash(documentDto.Content);
        }

        var folder = groupDto.Name;
        var zipFolderName = $"{folder}.zip";
        groupDto.Name = zipFolderName;
        groupDto.Identifier = Guid.NewGuid();
        groupDto.OperationId = (int)OperationType.Add;
        groupDto.Content = ZipHelper.ZipGroup(groupDto);
    }

    public async Task<GroupDto> CreateAsync(GroupInputDto input)
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

                    // TODO cut only div  //div[@class='container']

                    break;
            }

            var name = this.CreateNameFromUrl(input.Url);
            var documentDto = await this.CreateDocumentAsync(response, input.Url);

            return new GroupDto
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
        return $"{uri.Host}_{string.Join("_", uri.Segments.Where(x => x != "/").Select(x => x.Replace("/", string.Empty)))}".ToLower();
    }

    public bool CheckForUpdate(GroupDto groupDto, GroupDto dbGroupDto)
    {
        var isUpdated = false;
        foreach (var documentDto in groupDto.Documents)
        {
            var dbDocumentDto = dbGroupDto
                .Documents
                .Where(x => x.Name == documentDto.Name)
                .FirstOrDefault();

            if (dbDocumentDto == null)
            {
                isUpdated = true;
            }
            else
            {
                if (documentDto.MD5 != dbDocumentDto.MD5 || documentDto.Format != dbDocumentDto.Format)
                {
                    isUpdated = true;
                }
            }
        }

        foreach (var document in dbGroupDto.Documents)
        {
            if (!groupDto.Documents.Where(d => d.Name == document.Name).Any())
            {
                isUpdated = true;
            }
        }

        return isUpdated;
    }

    public void Update(GroupDto groupDto, GroupDto dbGroupDto)
    {
        dbGroupDto.OperationId = (int)OperationType.Update;
        dbGroupDto.Content = groupDto.Content;

        //foreach (var document in groupDto.Documents)
        //{
        //    if (document.Operation == (int)OperationType.Add)
        //    {
        //        document.Operation = (int)OperationType.Add;
        //        group.Documents.Add(document);
        //    }

        //    if (document.Operation == (int)OperationType.Update)
        //    {
        //        var doc = group.Documents.Single(d => d.Name == document.Name);
        //        doc.Operation = (int)OperationType.Update;
        //        doc.Format = document.Format;
        //        doc.Url = document.Url;
        //        doc.MD5 = document.MD5;
        //    }

        //    if (document.Operation == (int)OperationType.None)
        //    {
        //        var doc = group.Documents.Single(d => d.Name == document.Name);
        //        doc.Operation = (int)OperationType.None;
        //    }
        //}

        //foreach (var document in oldGroup.Documents)
        //{
        //    if (document.Operation == (int)OperationType.Delete)
        //    {
        //        var doc = group.Documents.FirstOrDefault(d => d.Name == document.Name);
        //        if (doc != null)
        //        {
        //            doc.Operation = document.Operation;
        //        }
        //    }
        //}
    }
}