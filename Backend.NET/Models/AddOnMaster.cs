using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("add_on_master")]
public class AddOnMaster
{
    [Key]
    [Column("add_on_id")]
    public int AddOnId { get; set; }

    [Column("add_on_name")]
    public string AddOnName { get; set; }

    [Column("add_on_daily_rate")]
    public double AddonDailyRate { get; set; }

    [Column("rate_valid_until")]
    public DateTime? RateValidUntil { get; set; }
}
