namespace CrawlerStorage.Services;

using System;
using System.Text;
using System.Threading.Tasks;

using CrawlerStorage.Common.Helpers;
using CrawlerStorage.Data.Models.Dtos.Documents;
using CrawlerStorage.Data.Models.Dtos.Groups;
using CrawlerStorage.Data.Models.Enumerations;
using CrawlerStorage.Services.Intrefaces;

using HtmlAgilityPack;

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
            documentDto.OperationId = (int)OperationType.Add;
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
            var content = await response.Content.ReadAsByteArrayAsync();
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

                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(responseAsString);

                    var html = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='container']");
                    if (html != null)
                    {
                        content = Encoding.UTF8.GetBytes(html.OuterHtml);
                    }

                    break;
            }

            var name = this.CreateNameFromUrl(input.Url);
            var documentDto = this.CreateDocument(response, content, input.Url);

            return new GroupDto
            {
                Name = name,
                CrawlerId = input.CrawlerId,
                Documents = [documentDto]
            };
        }

        return null;
    }

    private DocumentDto CreateDocument(HttpResponseMessage response, byte[] content, string url)
    {
        var name = this.CreateNameFromUrl(url);
        var documentDto = new DocumentDto
        {
            Name = name.ToLower(),
            Format = response.Content.Headers.ContentType?.MediaType,
            Url = url,
            Content = content,
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
                documentDto.OperationId = (int)OperationType.Add;
                isUpdated = true;
            }
            else
            {
                if (documentDto.MD5 != dbDocumentDto.MD5 || documentDto.Format != dbDocumentDto.Format)
                {
                    documentDto.OperationId = (int)OperationType.Update;
                    dbDocumentDto.OperationId = (int)OperationType.Update;
                    isUpdated = true;
                }
            }
        }

        foreach (var documentDto in dbGroupDto.Documents)
        {
            if (!groupDto.Documents.Where(d => d.Name == documentDto.Name).Any())
            {
                documentDto.OperationId = (int)OperationType.Delete;
                isUpdated = true;
            }
        }

        return isUpdated;
    }

    public void Update(GroupDto groupDto, GroupDto dbGroupDto)
    {
        dbGroupDto.OperationId = (int)OperationType.Update;
        dbGroupDto.Content = groupDto.Content;

        foreach (var documentDto in groupDto.Documents)
        {
            if (documentDto.OperationId == (int)OperationType.Add)
            {
                documentDto.OperationId = (int)OperationType.Add;
                dbGroupDto.Documents.Add(documentDto);
            }

            if (documentDto.OperationId == (int)OperationType.Update)
            {
                var doc = dbGroupDto.Documents.Single(d => d.Name == documentDto.Name);
                doc.OperationId = (int)OperationType.Update;
                doc.Format = documentDto.Format;
                doc.Url = documentDto.Url;
                doc.MD5 = documentDto.MD5;
            }

            if (documentDto.OperationId == (int)OperationType.None)
            {
                var doc = dbGroupDto.Documents.Single(d => d.Name == documentDto.Name);
                doc.OperationId = (int)OperationType.None;
            }
        }

        foreach (var documentDto in groupDto.Documents)
        {
            if (documentDto.OperationId == (int)OperationType.Delete)
            {
                var doc = groupDto.Documents.FirstOrDefault(d => d.Name == documentDto.Name);
                if (doc != null)
                {
                    doc.OperationId = documentDto.OperationId;
                }
            }
        }
    }
}