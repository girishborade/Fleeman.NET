using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("user")]
public class User
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("username")]
    public string? Username { get; set; }

    [Column("password")]
    public string? Password { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("role")]
    public string? Role { get; set; } // Storing enum as string. Ideally use Enum datatype with conversion.

    [ForeignKey("Hub")]
    [Column("hub_id")]
    public int? HubId { get; set; }
    public HubMaster? Hub { get; set; }

    [Column("reset_token")]
    public string? ResetToken { get; set; }

    [Column("reset_token_expiry")]
    public DateTime? ResetTokenExpiry { get; set; }


}
