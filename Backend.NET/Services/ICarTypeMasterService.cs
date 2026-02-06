using System.Collections.Generic;
using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Services;

public interface ICarTypeMasterService
{
    List<CarTypeMaster> GetAllCarTypes();
}
