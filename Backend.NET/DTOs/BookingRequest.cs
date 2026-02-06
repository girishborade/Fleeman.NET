using System;
using System.Collections.Generic;

namespace FleetManagementSystem.Api.DTOs;

public class BookingRequest
{
    public int CustomerId { get; set; }
    public int CarId { get; set; }
    public int PickupHubId { get; set; }
    public int ReturnHubId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<int> AddOnIds { get; set; }
    public string? Email { get; set; }
    public long? CarTypeId { get; set; }
}
