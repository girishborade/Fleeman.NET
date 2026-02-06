using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("invoice_detail_table")]
public class InvoiceDetailTable
{
    [Key]
    [Column("invdtl_id")]
    public long InvdtlId { get; set; }

    [ForeignKey("Invoice")]
    [Column("invoice_id")]
    public long? InvoiceId { get; set; }
    public InvoiceHeaderTable Invoice { get; set; }

    [ForeignKey("AddOn")]
    [Column("addon_id")]
    public int? AddOnId { get; set; }
    public AddOnMaster AddOn { get; set; }

    [Column("addon_amt")]
    public double AddonAmt { get; set; }
}
