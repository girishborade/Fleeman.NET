using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.Services;
using FleetManagementSystem.Api.DTOs;
using System.Collections.Generic;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/v1")]
public class SupportController : ControllerBase
{
    private readonly ISupportService _supportService;
    public SupportController(ISupportService supportService) => _supportService = supportService;

    [HttpPost("support-tickets")]
    public ActionResult<SupportTicket> CreateTicket([FromBody] SupportTicket ticket) => Ok(_supportService.CreateTicket(ticket));
    
    [HttpGet("support-tickets")]
    public ActionResult<List<SupportTicket>> GetAllTickets() => Ok(_supportService.GetAllTickets());
}

[ApiController]
[Route("api/v1")]
public class CustomerExistsController : ControllerBase
{
    private readonly ICheckCustomerExistsService _service;
    public CustomerExistsController(ICheckCustomerExistsService service) => _service = service;

    [HttpGet("customers/exists/{email}")]
    public ActionResult<bool> CheckExists(string email)
    {
        return Ok(_service.CustomerExists(email));
    }
}

[ApiController]
[Route("api/v1")]
public class GetCarDetailsFromBookingController : ControllerBase
{
    private readonly IGetCarDetailsFromBookingService _service;
    public GetCarDetailsFromBookingController(IGetCarDetailsFromBookingService service) => _service = service;

    [HttpGet("booking/{bookingId}/car")]
    public ActionResult<CarMaster> GetCar(long bookingId)
    {
        var car = _service.GetCarDetails(bookingId);
        if (car != null) return Ok(car);
        return NotFound();
    }
}

[ApiController]
[Route("api/v1")]
public class LocaleController : ControllerBase
{
    private readonly ILocaleService _service;
    public LocaleController(ILocaleService service) => _service = service;
    
    [HttpGet("locale/{lang}")]
    public ActionResult<string> GetLocale(string lang) => Ok(_service.GetLocaleData(lang));
}
