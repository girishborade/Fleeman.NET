using System;
using System.Collections.Generic;
using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Services;

public interface ICarService
{
    List<object[]> GetCarsByHubAddress(string hubAddress);
    List<CarMaster> GetAvailableCars(int hubId, DateTime startDate, DateTime endDate, long? carTypeId);
}
