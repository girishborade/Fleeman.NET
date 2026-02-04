using System;

namespace FleetManagementSystem.Api.DTOs;

public class AddOnDetailDto
{
    public long AddOnId { get; set; }
    public string AddOnName { get; set; }
    public double DailyRate { get; set; }
    public int Quantity { get; set; } // For items like child seats
}
