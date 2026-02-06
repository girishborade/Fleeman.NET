namespace FleetManagementSystem.Api.DTOs;

public class HandoverRequest
{
    public long BookingId { get; set; }
    public int? CarId { get; set; }
    public string? FuelStatus { get; set; } // "1/4", "1/2", "3/4", "Full"
    public string? Notes { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
