using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("city_master")]
public class CityMaster
{
    [Key]
    [Column("city_id")]
    public int CityId { get; set; }

    [Column("city_name")]
    public string CityName { get; set; }

    [ForeignKey("State")]
    [Column("state_id")]
    public int StateId { get; set; }

    public StateMaster State { get; set; }
}
