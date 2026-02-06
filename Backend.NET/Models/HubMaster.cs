using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("hub_master")]
public class HubMaster
{
    [Key]
    [Column("hub_id")]
    public int HubId { get; set; }

    [Column("hub_name")]
    public string? HubName { get; set; }

    [Column("hub_address_and_details", TypeName = "TEXT")]
    public string? HubAddressAndDetails { get; set; }

    [Column("contact_number")]
    public long ContactNumber { get; set; }

    [ForeignKey("City")]
    [Column("city_id")]
    public int? CityId { get; set; }
    public CityMaster? City { get; set; }

    [ForeignKey("State")]
    [Column("state_id")]
    public int? StateId { get; set; }
    public StateMaster? State { get; set; }
}
