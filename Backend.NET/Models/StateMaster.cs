using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("state_master")]
public class StateMaster
{
    [Key]
    [Column("state_id")]
    public int StateId { get; set; }

    [Column("state_name")]
    public string StateName { get; set; }
}
