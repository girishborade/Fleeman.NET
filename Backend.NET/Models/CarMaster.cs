using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("car_master")]
public class CarMaster
{
    [Key]
    [Column("car_id")]
    public int CarId { get; set; }

    [ForeignKey("CarType")]
    [Column("cartype_id")]
    public long? CarTypeId { get; set; }
    public CarTypeMaster? CarType { get; set; }

    [Column("car_name")]
    public string? CarName { get; set; }

    [Column("number_plate")]
    public string? NumberPlate { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Column("mileage")]
    public double Mileage { get; set; }

    [ForeignKey("Hub")]
    [Column("hub_id")]
    public int? HubId { get; set; }
    public HubMaster? Hub { get; set; }

    [Column("is_available")]
    public string? IsAvailable { get; set; }

    [Column("maintenance_due_date")]
    public DateTime? MaintenanceDueDate { get; set; }

    [Column("image_path")]
    public string? ImagePath { get; set; }

    public bool IsActuallyAvailable()
    {
        return IsAvailable == "Y" || IsAvailable == "YES" || IsAvailable == "True";
    }
}
