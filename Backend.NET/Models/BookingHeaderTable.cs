using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("booking_header_table")]
public class BookingHeaderTable
{
    [Key]
    [Column("booking_id")]
    public long BookingId { get; set; }

    [Column("booking_date")]
    public DateTime? BookingDate { get; set; }

    [Column("confirmation_number")]
    public string? ConfirmationNumber { get; set; }

    [Column("booking_status")]
    public string? BookingStatus { get; set; }

    [ForeignKey("Customer")]
    [Column("cust_id")]
    public int CustomerId { get; set; }
    public CustomerMaster? Customer { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [ForeignKey("PickupHub")]
    [Column("pickup_location_id")]
    public int? PickupHubId { get; set; }
    public HubMaster? PickupHub { get; set; }

    [ForeignKey("ReturnHub")]
    [Column("return_hub_id")]
    public int? ReturnHubId { get; set; }
    public HubMaster? ReturnHub { get; set; }

    [ForeignKey("CarType")]
    [Column("cartype_id")]
    public long? CarTypeId { get; set; }
    public CarTypeMaster? CarType { get; set; }

    [ForeignKey("Car")]
    [Column("car_id")]
    public int? CarId { get; set; }
    public CarMaster? Car { get; set; }

    [Column("first_name")]
    public string? FirstName { get; set; }

    [Column("last_name")]
    public string? LastName { get; set; }

    [Column("address")]
    public string? Address { get; set; }

    [Column("state")]
    public string? State { get; set; }

    [Column("pin")]
    public string? Pin { get; set; }

    [Column("daily_rate")]
    public double? DailyRate { get; set; }

    [Column("weekly_rate")]
    public double? WeeklyRate { get; set; }

    [Column("monthly_rate")]
    public double? MonthlyRate { get; set; }

    [Column("email_id")]
    public string? EmailId { get; set; }

    [Column("book_car")]
    public string? Bookcar { get; set; }

    [Column("pickup_time")]
    public DateTime? PickupTime { get; set; }

    [Column("pickup_fuel_status")]
    public string? PickupFuelStatus { get; set; }

    [Column("pickup_condition")]
    public string? PickupCondition { get; set; }

    [Column("return_time")]
    public DateTime? ReturnTime { get; set; }

    [Column("return_fuel_status")]
    public string? ReturnFuelStatus { get; set; }

    [Column("return_condition")]
    public string? ReturnCondition { get; set; }
}
