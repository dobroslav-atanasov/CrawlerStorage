namespace CrawlerStorage.Data.Common.Models;

using System.ComponentModel.DataAnnotations;

using CrawlerStorage.Data.Common.Interfaces;

public abstract class BaseEntity<TKey> : ICheckableEntity
{
    [Key]
    public TKey Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }
}