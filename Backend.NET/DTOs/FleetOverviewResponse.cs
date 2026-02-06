namespace FleetManagementSystem.Api.DTOs;

public class FleetOverviewResponse
{
    public List<HubFleetData> Hubs { get; set; } = new();
    public FleetStatistics Statistics { get; set; } = new();
}

public class HubFleetData
{
    public int HubId { get; set; }
    public string HubName { get; set; } = string.Empty;
    public string? CityName { get; set; }
    public List<CarStatusData> Cars { get; set; } = new();
    public int TotalCars { get; set; }
    public int AvailableCars { get; set; }
    public int RentedCars { get; set; }
    public int MaintenanceCars { get; set; }
}

public class CarStatusData
{
    public int CarId { get; set; }
    public string Model { get; set; } = string.Empty;
    public string? CarType { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Available", "Rented", "Maintenance"
    public decimal? DailyRate { get; set; }
    public string? ImagePath { get; set; }
    
    // If rented, include booking info
    public RentalInfo? CurrentRental { get; set; }
}

public class RentalInfo
{
    public long BookingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? PickupTime { get; set; }
}

public class FleetStatistics
{
    public int TotalCars { get; set; }
    public int TotalAvailable { get; set; }
    public int TotalRented { get; set; }
    public int TotalMaintenance { get; set; }
    public decimal UtilizationRate { get; set; }
}
