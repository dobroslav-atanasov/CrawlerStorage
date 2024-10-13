namespace CrawlerStorage.Services;

using System;
using System.Collections.Generic;
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

    public void Process(GroupDto groupDto)
    {
        foreach (var documentDto in groupDto.Documents)
        {
            documentDto.Identifier = Guid.NewGuid();
            documentDto.MD5 = MD5Helper.Hash(documentDto.Content);
            documentDto.Operation = OperationType.Add.ToString();
        }

        var folder = groupDto.Name;
        var zipFolderName = $"{folder}.zip";
        groupDto.Name = zipFolderName;
        groupDto.Identifier = Guid.NewGuid();
        groupDto.Operation = OperationType.Add.ToString();
        groupDto.Content = ZipHelper.ZipGroup(groupDto);
    }

    public async Task<GroupDto> CreateAsync(GroupInputDto input)
    {
        var response = await this.httpClient.GetAsync(input.Url);

        if (response.IsSuccessStatusCode)
        {
            var content = await this.ReadContentAsync(response, input.CrawlerId, input.Url);
            var name = this.CreateNameFromUrl(input.Url);
            var documentDto = this.CreateDocument(response, content, input.Url);

            var groupDto = new GroupDto
            {
                Name = name,
                CrawlerId = input.CrawlerId,
                Documents = [documentDto]
            };

            if (input.DocumentUrls.Count > 0)
            {
                var documentDtos = await this.CreateDocumentsAsync(input);
                groupDto.Documents.AddRange(documentDtos);
            }

            return groupDto;
        }

        return null;
    }

    private async Task<byte[]> ReadContentAsync(HttpResponseMessage response, int crawlerId, string url)
    {
        var content = await response.Content.ReadAsByteArrayAsync();
        switch (crawlerId)
        {
            case 1:
                var responseAsString = await response.Content.ReadAsStringAsync();
                while (responseAsString == "Rate Limit Exceeded")
                {
                    await Task.Delay(3000);
                    response = await this.httpClient.GetAsync(url);
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

        return content;
    }

    private async Task<List<DocumentDto>> CreateDocumentsAsync(GroupInputDto input)
    {
        var list = new List<DocumentDto>();

        foreach (var url in input.DocumentUrls)
        {
            var response = await this.httpClient.GetAsync(url);
            var content = await this.ReadContentAsync(response, input.CrawlerId, url);
            var name = this.CreateNameFromUrl(url);
            var documentDto = this.CreateDocument(response, content, url);

            list.Add(documentDto);
        }

        return list;
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

    public string CreateNameFromUrl(string url)
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
                documentDto.Operation = OperationType.Add.ToString();
                isUpdated = true;
            }
            else
            {
                if (documentDto.MD5 != dbDocumentDto.MD5 || documentDto.Format != dbDocumentDto.Format)
                {
                    documentDto.Operation = OperationType.Update.ToString();
                    dbDocumentDto.Operation = OperationType.Update.ToString();
                    isUpdated = true;
                }
            }
        }

        foreach (var documentDto in dbGroupDto.Documents)
        {
            if (!groupDto.Documents.Where(d => d.Name == documentDto.Name).Any())
            {
                documentDto.Operation = OperationType.Delete.ToString();
                isUpdated = true;
            }
        }

        return isUpdated;
    }

    public void Update(GroupDto groupDto, GroupDto dbGroupDto)
    {
        dbGroupDto.Operation = OperationType.Update.ToString();
        dbGroupDto.Content = groupDto.Content;

        foreach (var documentDto in groupDto.Documents)
        {
            if (documentDto.Operation == OperationType.Add.ToString())
            {
                documentDto.Operation = OperationType.Add.ToString();
                dbGroupDto.Documents.Add(documentDto);
            }

            if (documentDto.Operation == OperationType.Update.ToString())
            {
                var doc = dbGroupDto.Documents.Single(d => d.Name == documentDto.Name);
                doc.Operation = OperationType.Update.ToString();
                doc.Format = documentDto.Format;
                doc.Url = documentDto.Url;
                doc.MD5 = documentDto.MD5;
            }

            if (documentDto.Operation == OperationType.None.ToString())
            {
                var doc = dbGroupDto.Documents.Single(d => d.Name == documentDto.Name);
                doc.Operation = OperationType.None.ToString();
            }
        }

        foreach (var documentDto in groupDto.Documents)
        {
            if (documentDto.Operation == OperationType.Delete.ToString())
            {
                var doc = groupDto.Documents.FirstOrDefault(d => d.Name == documentDto.Name);
                if (doc != null)
                {
                    doc.Operation = documentDto.Operation;
                }
            }
        }
    }
}