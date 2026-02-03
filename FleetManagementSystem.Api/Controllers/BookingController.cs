using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FleetManagementSystem.Api.DTOs;
using FleetManagementSystem.Api.Services;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("booking")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // Moved GetBooking to the bottom to avoid potential route conflicts with literal paths like "return", "all", etc.

    [HttpGet("heartbeat")]
    public IActionResult Heartbeat() => Ok("Booking Controller is Running - Version 3 (Get Fix)");

    [HttpGet("user/{email}")]
    public ActionResult<List<BookingResponse>> GetBookingsByUser(string email)
    {
        return Ok(_bookingService.GetBookingsByEmail(email));
    }

    [HttpGet("all")]
    public ActionResult<List<BookingResponse>> GetAllBookings()
    {
        return Ok(_bookingService.GetAllBookings());
    }

    [HttpPost("create")]
    public ActionResult<BookingResponse> CreateBooking([FromBody] BookingRequest request)
    {
        try
        {
            return Ok(_bookingService.CreateBooking(request));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("handover/{id}")]
    public ActionResult<BookingResponse> HandoverCar([FromRoute] long id)
    {
        // Delegating to ProcessHandover wrapper logic if needed, but ProcessHandover(HandoverRequest) is main 
        // Java code: handoverCar(Long) -> calls processHandover(new HandoverRequest(id))
        var req = new HandoverRequest { BookingId = id };
        try
        {
            return Ok(_bookingService.ProcessHandover(req));
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("process-handover")]
    public ActionResult<BookingResponse> ProcessHandover([FromBody] HandoverRequest request)
    {
        try
        {
            return Ok(_bookingService.ProcessHandover(request));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("return")]
    public ActionResult<BookingResponse> ReturnCar([FromBody] ReturnRequest request)
    {
        try
        {
            return Ok(_bookingService.ReturnCar(request));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("cancel/{id}")]
    public ActionResult<BookingResponse> CancelBooking([FromRoute] long id)
    {
        try
        {
            return Ok(_bookingService.CancelBooking(id));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("modify/{id}")]
    public ActionResult<BookingResponse> ModifyBooking([FromRoute] long id, [FromBody] BookingRequest request)
    {
        try
        {
             return Ok(_bookingService.ModifyBooking(id, request));
        }
        catch (Exception ex)
        {
             return BadRequest(ex.Message);
        }
    }

    [HttpPost("storeDates")]
    public ActionResult<string> StoreBookingDates([FromQuery] string start_date, [FromQuery] string end_date, [FromQuery] int cust_id)
    {
        // Stub as in Java
        // initialDateService.storeTempDateandTime(...)
        return Ok("success");
    }

    [HttpGet("get/{id}")]
    public ActionResult<BookingResponse> GetBooking([FromRoute] string id)
    {
        try
        {
            var response = _bookingService.GetBooking(id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            // Consistent error reporting
            return NotFound(ex.Message);
        }
    }
}
