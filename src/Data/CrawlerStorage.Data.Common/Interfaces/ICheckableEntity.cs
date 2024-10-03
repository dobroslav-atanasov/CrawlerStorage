namespace CrawlerStorage.Data.Common.Interfaces;

public interface ICheckableEntity
{
    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }
}