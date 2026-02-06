using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("vendors")]
public class Vendor
{
    [Key]
    [Column("vendor_id")]
    public long VendorId { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("type")]
    public string? Type { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("api_url")]
    public string? ApiUrl { get; set; }
}
