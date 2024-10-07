namespace CrawlerStorage.Data.Models.Dtos.Groups;

using CrawlerStorage.Data.Models.Dtos.Documents;

public class GroupDto
{
    public int Id { get; set; }

    public Guid Identifier { get; set; }

    public string Name { get; set; }

    public int CrawlerId { get; set; }

    public string Operation { get; set; }

    public byte[] Content { get; set; }

    public List<DocumentDto> Documents { get; set; } = [];
}