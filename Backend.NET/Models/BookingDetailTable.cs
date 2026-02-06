using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("booking_detail_table")]
public class BookingDetailTable
{
    [Key]
    [Column("booking_detail_id")]
    public long BookingDetailId { get; set; }

    [ForeignKey("Booking")]
    [Column("booking_id")]
    public long? BookingId { get; set; }
    public BookingHeaderTable Booking { get; set; }

    [ForeignKey("AddOn")]
    [Column("addon_id")]
    public int? AddOnId { get; set; }
    public AddOnMaster AddOn { get; set; }

    [Column("addon_rate")]
    public double? AddonRate { get; set; }
}
