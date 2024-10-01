namespace CrawlerStorage.Data.Models.DbEntities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CrawlerStorage.Data.Common.Models;

[Table("Logs")]
public class Log : BaseEntity<int>
{
    //[Required]
    //public DateTime LogDate { get; set; }

    [Required]
    [Column(TypeName = "UUID")]
    public Guid Identifier { get; set; }

    [Required]
    public int OperationId { get; set; }
    public virtual Operation Operation { get; set; }

    [Required]
    public int CrawlerId { get; set; }
    public virtual Crawler Crawler { get; set; }

    [MaxLength(5000)]
    public string Message { get; set; }
}