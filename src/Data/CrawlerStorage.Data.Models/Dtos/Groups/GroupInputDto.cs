namespace CrawlerStorage.Data.Models.Dtos.Groups;

public class GroupInputDto
{
    public string Url { get; set; }

    public int CrawlerId { get; set; }

    public List<string> DocumentUrls { get; set; } = [];
}