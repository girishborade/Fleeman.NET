using System;

namespace FleetManagementSystem.Api.DTOs;

public class ReturnCarMasterDetailsDTO
{
    public class CarDetailsDTO
    {
        public string Status { get; set; }
        public string CarName { get; set; }
        public string IsAvailable { get; set; }
        public DateTime MaintenanceDueDate { get; set; }
        public double Mileage { get; set; }
        public string NumberPlate { get; set; }
    }
}
