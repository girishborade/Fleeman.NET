using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("support_tickets")]
public class SupportTicket
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("subject")]
    public string Subject { get; set; }

    [Column("message", TypeName = "TEXT")]
    public string Message { get; set; }

    [Column("status")]
    public string Status { get; set; }

    [Column("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
