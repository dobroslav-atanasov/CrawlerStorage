namespace CrawlerStorage.Data.Models.Dtos.Groups;

using CrawlerStorage.Data.Models.Dtos.Documents;

public class GroupDto
{
    public Guid Identifier { get; set; }

    public string Name { get; set; }

    public int CrawlerId { get; set; }

    public int OperationId { get; set; }

    public byte[] Content { get; set; }

    public ICollection<DocumentDto> Documents { get; set; } = [];
}