using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("airport_master")]
public class AirportMaster
{
    [Key]
    [Column("airport_id")]
    public int AirportId { get; set; }

    [Column("airport_name")]
    public string AirportName { get; set; }

    [Column("airport_code")]
    public string AirportCode { get; set; }

    [ForeignKey("City")]
    [Column("city_id")]
    public int? CityId { get; set; }
    public CityMaster City { get; set; }

    [ForeignKey("State")]
    [Column("state_id")]
    public int? StateId { get; set; }
    public StateMaster State { get; set; }

    [ForeignKey("Hub")]
    [Column("hub_id")]
    public int? HubId { get; set; }
    public HubMaster Hub { get; set; }
}
