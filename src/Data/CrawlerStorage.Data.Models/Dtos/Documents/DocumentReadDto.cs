namespace CrawlerStorage.Data.Models.Dtos.Documents;

public class DocumentReadDto
{
    public int Id { get; set; }

    public Guid Identifier { get; set; }

    public string Name { get; set; }

    public string Operation { get; set; }
}