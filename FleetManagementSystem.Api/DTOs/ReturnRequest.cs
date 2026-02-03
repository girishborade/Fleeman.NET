using System;

namespace FleetManagementSystem.Api.DTOs;

public class ReturnRequest
{
    public long BookingId { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? FuelStatus { get; set; }
    public string? Notes { get; set; }
}
