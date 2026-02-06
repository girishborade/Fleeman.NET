using System.Collections.Generic;
using System.Linq;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Services;

public class CarTypeMasterService : ICarTypeMasterService
{
    private readonly ApplicationDbContext _context;

    public CarTypeMasterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<CarTypeMaster> GetAllCarTypes()
    {
        return _context.CarTypes.ToList();
    }
}
