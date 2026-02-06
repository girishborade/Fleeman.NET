using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetManagementSystem.Api.Models;

[Table("customer_master")]
public class CustomerMaster
{
    [Key]
    [Column("cust_id")]
    public int CustId { get; set; }

    [Column("membership_id")]
    public string? MembershipId { get; set; }

    [Column("first_name")]
    public string? FirstName { get; set; }

    [Column("last_name")]
    public string? LastName { get; set; }

    [Column("address_line1")]
    public string? AddressLine1 { get; set; }

    [Column("address_line2")]
    public string? AddressLine2 { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("city")]
    public string? City { get; set; }

    [Column("pincode")]
    public string? Pincode { get; set; }

    [Column("phone_number")]
    public string? PhoneNumber { get; set; }

    [Column("mobile_number")]
    public string? MobileNumber { get; set; }

    [Column("credit_card_type")]
    public string? CreditCardType { get; set; }

    [Column("credit_card_number")]
    public string? CreditCardNumber { get; set; }

    [Column("driving_license_number")]
    public string? DrivingLicenseNumber { get; set; }

    [Column("idp_number")]
    public string? IdpNumber { get; set; }

    [Column("issued_bydl")]
    public string? IssuedByDL { get; set; }

    [Column("valid_throughdl")]
    public DateTime? ValidThroughDL { get; set; }

    [Column("passport_number")]
    public string? PassportNumber { get; set; }

    [Column("passport_valid_through")]
    public DateTime? PassportValidThrough { get; set; }

    [Column("passport_issued_by")]
    public string? PassportIssuedBy { get; set; }

    [Column("passport_valid_from")]
    public DateTime? PassportValidFrom { get; set; }

    [Column("passport_issue_date")]
    public DateTime? PassportIssueDate { get; set; }

    [Column("date_of_birth")]
    public DateTime? DateOfBirth { get; set; }
}
