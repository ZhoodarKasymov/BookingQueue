using System.ComponentModel.DataAnnotations.Schema;

namespace BookingQueue.Common.Models;

[Table("services")]
public class Services
{
    [Column("id")]
    public long? Id { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("service_prefix")]
    public string? ServicePrefix { get; set; }

    [Column("deleted")]
    public DateOnly? Deleted { get; set; }
}