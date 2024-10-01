namespace CrawlerStorage.Data.Models.Dtos.Groups;

using CrawlerStorage.Data.Models.Dtos.Documents;

public class GroupCreateDto
{
    public string Name { get; set; }

    public int CrawlerId { get; set; }

    public List<DocumentDto> Documents { get; set; } = [];
}