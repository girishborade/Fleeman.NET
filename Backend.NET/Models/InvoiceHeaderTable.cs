using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("invoice_header_table")]
public class InvoiceHeaderTable
{
    [Key]
    [Column("invoice_id")]
    public long InvoiceId { get; set; }

    [Column("date")]
    public DateTime? Date { get; set; }

    [ForeignKey("Booking")]
    [Column("booking_id")]
    public long? BookingId { get; set; }
    public BookingHeaderTable Booking { get; set; }

    [ForeignKey("Customer")]
    [Column("cust_id")]
    public int? CustomerId { get; set; }
    public CustomerMaster Customer { get; set; }

    [Column("handover_date")]
    public DateTime? HandoverDate { get; set; }

    [ForeignKey("Car")]
    [Column("car_id")]
    public int? CarId { get; set; }
    public CarMaster Car { get; set; }

    [Column("return_date")]
    public DateTime? ReturnDate { get; set; }

    [Column("rental_amt")]
    public double RentalAmt { get; set; }

    [Column("total_addon_amt")]
    public double TotalAddonAmt { get; set; }

    [Column("total_amt")]
    public double TotalAmt { get; set; }

    [Column("customer_details")]
    public string? CustomerDetails { get; set; }

    [Column("rate")]
    public string? Rate { get; set; }
}
