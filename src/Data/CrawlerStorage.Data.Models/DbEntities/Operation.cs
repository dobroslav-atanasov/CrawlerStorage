namespace CrawlerStorage.Data.Models.DbEntities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CrawlerStorage.Data.Common.Models;

[Table("Operations")]
public class Operation : BaseEntity<int>
{
    [Required]
    [MaxLength(10)]
    public string Name { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = [];

    public virtual ICollection<Group> Groups { get; set; } = [];
}