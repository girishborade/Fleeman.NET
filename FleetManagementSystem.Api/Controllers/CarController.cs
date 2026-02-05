using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.Services;
using FleetManagementSystem.Api.DTOs;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IExcelUploadService _excelUploadService;

    public CarController(ICarService carService, IExcelUploadService excelUploadService)
    {
        _carService = carService;
        _excelUploadService = excelUploadService;
    }

    [HttpPost("cars/upload")]
    public IActionResult UploadFile(IFormFile file)
    {
        string message = "";
        try
        {
            _excelUploadService.SaveCars(file);
            message = "Uploaded the file successfully: " + file.FileName;
            return Ok(new MessageResponse(message));
        }
        catch (Exception)
        {
            message = "Could not upload the file: " + file.FileName + "!";
            return StatusCode(StatusCodes.Status417ExpectationFailed, new MessageResponse(message));
        }
    }

    [HttpGet("cars")]
    public ActionResult<List<object[]>> GetCarDetailsByHubAddress([FromQuery] string hubAddress)
    {
        var cars = _carService.GetCarsByHubAddress(hubAddress);
        return Ok(cars);
    }

    [HttpGet("cars/available")]
    public ActionResult<List<CarMaster>> GetAvailableCars(
            [FromQuery] int hubId,
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] long? carTypeId = null)
    {
        // Parsing dates manually as in Java
        if (DateTime.TryParse(startDate, out DateTime start) && DateTime.TryParse(endDate, out DateTime end))
        {
             var cars = _carService.GetAvailableCars(hubId, start, end, carTypeId);
             return Ok(cars);
        }
        return BadRequest("Invalid date format");
    }
}
