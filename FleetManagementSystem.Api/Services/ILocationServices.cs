using System.Collections.Generic;
using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Services;

public interface IStateService
{
    List<StateMaster> GetAllStates();
}

public interface ICityService
{
    List<CityMaster> GetAllCities();
    List<CityMaster> GetCitiesByStateId(int stateId);
}

public interface IHubService
{
    List<HubMaster> GetAllHubs();
    List<HubMaster> GetHubsByCityId(int cityId);
    HubMaster GetHubById(int hubId);
    List<HubMaster> SearchHubsByAirportCode(string airportCode);
}

public interface IAirportService
{
    List<AirportMaster> GetAllAirports();
    List<AirportMaster> GetAirportsByStateId(int stateId);
}
