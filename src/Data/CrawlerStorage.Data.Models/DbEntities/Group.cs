namespace CrawlerStorage.Data.Models.DbEntities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CrawlerStorage.Data.Common.Models;

[Table("Groups")]
public class Group : BaseEntity<int>
{
    [Required]
    [Column(TypeName = "UUID")]
    public Guid Identifier { get; set; }

    [Required]
    [MaxLength(250)]
    public string Name { get; set; }

    //[Required]
    //public DateTime Date { get; set; }

    [Required]
    public int CrawlerId { get; set; }
    public virtual Crawler Crawler { get; set; }

    [Required]
    public int OperationId { get; set; }
    public virtual Operation Operation { get; set; }

    [Column(TypeName = "BYTEA")]
    public byte[] Content { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = [];
}