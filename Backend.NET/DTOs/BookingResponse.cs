using System;
using System.Collections.Generic;

namespace FleetManagementSystem.Api.DTOs;

public class BookingResponse
{
    public long BookingId { get; set; }
    public string ConfirmationNumber { get; set; }
    public string BookingStatus { get; set; }
    public string CustomerName { get; set; }
    public string Email { get; set; }
    public string CarName { get; set; }
    public string NumberPlate { get; set; }
    public string PickupHub { get; set; }
    public long? PickupHubId { get; set; }
    public string ReturnHub { get; set; }
    public long? CarTypeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public double? DailyRate { get; set; }
    public double? TotalAmount { get; set; }
    public double? TotalAddonAmount { get; set; }
    public List<string> SelectedAddOns { get; set; }
    public List<AddOnDetailDto> AddOnDetails { get; set; } // Detailed add-on info with rates
    
    // Detailed Customer Info
    public string MobileNumber { get; set; }
    public string DrivingLicenseNumber { get; set; }
    public string AddressLine1 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Pincode { get; set; }
}
