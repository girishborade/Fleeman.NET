using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.Services;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class CarTypeMasterController : ControllerBase
{
    private readonly ICarTypeMasterService _carTypeMasterService;

    public CarTypeMasterController(ICarTypeMasterService carTypeMasterService)
    {
        _carTypeMasterService = carTypeMasterService;
    }

    [HttpGet("car-types")]
    public ActionResult<List<CarTypeMaster>> GetAllCarTypes()
    {
        return Ok(_carTypeMasterService.GetAllCarTypes());
    }
}
