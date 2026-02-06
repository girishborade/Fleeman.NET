using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("car_type_master")]
public class CarTypeMaster
{
    [Key]
    [Column("cartype_id")]
    public long CarTypeId { get; set; }

    [Column("cartype_name")]
    public string? CarTypeName { get; set; }

    [Column("daily_rate")]
    public double DailyRate { get; set; }

    [Column("weekly_rate")]
    public double WeeklyRate { get; set; }

    [Column("monthly_rate")]
    public double MonthlyRate { get; set; }

    [Column("image_path")]
    public string? ImagePath { get; set; }
}
