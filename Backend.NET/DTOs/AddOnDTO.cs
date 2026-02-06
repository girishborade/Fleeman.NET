using System;

namespace FleetManagementSystem.Api.DTOs;

public class AddOnDTO
{
    public int AddOnId { get; set; }
    public string AddOnName { get; set; }
    public double AddonDailyRate { get; set; }
    public DateTime? RateValidUntil { get; set; }
}
