namespace CrawlerStorage.Data.Models.DbEntities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CrawlerStorage.Data.Common.Models;

[Table("Crawlers")]
public class Crawler : BaseEntity<int>
{
    [Required]
    [MaxLength(250)]
    public string Name { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = [];
}