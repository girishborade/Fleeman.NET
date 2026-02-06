using System.Collections.Generic;
using System.Linq;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Services;

public class StateService : IStateService
{
    private readonly ApplicationDbContext _context;
    public StateService(ApplicationDbContext context) => _context = context;
    public List<StateMaster> GetAllStates() => _context.States.ToList();
}

public class CityService : ICityService
{
    private readonly ApplicationDbContext _context;
    public CityService(ApplicationDbContext context) => _context = context;
    public List<CityMaster> GetAllCities() => _context.Cities.Include(c => c.State).ToList();
    public List<CityMaster> GetCitiesByStateId(int stateId) => _context.Cities.Where(c => c.StateId == stateId).Include(c => c.State).ToList();
}

public class HubService : IHubService
{
    private readonly ApplicationDbContext _context;
    public HubService(ApplicationDbContext context) => _context = context;
    public List<HubMaster> GetAllHubs() => _context.Hubs.Include(h => h.City).Include(h => h.City.State).ToList();
    public List<HubMaster> GetHubsByCityId(int cityId) => _context.Hubs.Where(h => h.CityId == cityId).Include(h => h.City).Include(h => h.City.State).ToList();
    public HubMaster GetHubById(int hubId) => _context.Hubs.Include(h => h.City).Include(h => h.City.State).FirstOrDefault(h => h.HubId == hubId);
    
    public List<HubMaster> SearchHubsByAirportCode(string airportCode)
    {
        var hubIds = _context.Airports
            .Where(a => a.AirportCode == airportCode && a.HubId != null)
            .Select(a => a.HubId)
            .ToList();

        return _context.Hubs
            .Where(h => hubIds.Contains(h.HubId))
            .Include(h => h.City)
            .Include(h => h.City.State)
            .ToList();
    }
}

public class AirportService : IAirportService
{
    private readonly ApplicationDbContext _context;
    public AirportService(ApplicationDbContext context) => _context = context;
    public List<AirportMaster> GetAllAirports() => _context.Airports.Include(a => a.City).Include(a => a.State).Include(a => a.Hub).ToList();
    public List<AirportMaster> GetAirportsByStateId(int stateId) => _context.Airports.Where(a => a.StateId == stateId).Include(a => a.City).Include(a => a.State).Include(a => a.Hub).ToList();
}
