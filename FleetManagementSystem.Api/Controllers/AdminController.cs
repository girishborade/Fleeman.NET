using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.DTOs;
using FleetManagementSystem.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace FleetManagementSystem.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IExcelUploadService _excelUploadService;

    public AdminController(ApplicationDbContext context, IExcelUploadService excelUploadService)
    {
        _context = context;
        _excelUploadService = excelUploadService;
    }

    // GET: api/admin/staff
    [HttpGet("staff")]
    public async Task<ActionResult<List<object>>> GetAllStaff()
    {
        var staffMembers = await _context.Users
            .Include(u => u.Hub)
                .ThenInclude(h => h.City)
            .Where(u => u.Role == "STAFF")
            .Select(u => new
            {
                id = u.Id,
                username = u.Username,
                email = u.Email,
                role = u.Role,
                hub = u.Hub != null ? new
                {
                    hubId = u.Hub.HubId,
                    hubName = u.Hub.HubName,
                    city = u.Hub.City != null ? new
                    {
                        cityId = u.Hub.City.CityId,
                        cityName = u.Hub.City.CityName
                    } : null
                } : null
            })
            .ToListAsync();

        return Ok(staffMembers);
    }

    // POST: api/admin/upload-rates
    [HttpPost("upload-rates")]
    public IActionResult UploadRates(IFormFile file)
    {
        try
        {
            _excelUploadService.SaveRates(file);
            return Ok(new MessageResponse("Rates uploaded successfully: " + file.FileName));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new MessageResponse("Failed to upload rates: " + ex.Message));
        }
    }

    // POST: api/admin/register-staff
    [HttpPost("register-staff")]
    public async Task<ActionResult> RegisterStaff([FromBody] StaffRegistrationRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username, email, and password are required." });
        }

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (existingUser != null)
        {
            return BadRequest(new { message = "Username or email already exists." });
        }

        // Hash password (in production, use proper password hashing like BCrypt)
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create new staff user
        var newStaff = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = hashedPassword,
            Role = "STAFF",
            HubId = request.Hub?.HubId
        };

        _context.Users.Add(newStaff);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Staff member registered successfully.", id = newStaff.Id });
    }

    // DELETE: api/admin/staff/{id}
    [HttpDelete("staff/{id}")]
    public async Task<ActionResult> DeleteStaff(int id)
    {
        var staff = await _context.Users.FindAsync(id);

        if (staff == null)
        {
            return NotFound(new { message = "Staff member not found." });
        }

        if (staff.Role != "STAFF")
        {
            return BadRequest(new { message = "Cannot delete non-staff users through this endpoint." });
        }

        _context.Users.Remove(staff);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Staff member deleted successfully." });
    }

    // GET: api/admin/fleet-overview
    [HttpGet("fleet-overview")]
    public async Task<ActionResult<FleetOverviewResponse>> GetFleetOverview([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        // Get all cars with hub and booking information
        var cars = await _context.Cars
            .Include(c => c.Hub)
                .ThenInclude(h => h!.City)
            .Include(c => c.CarType)
            .ToListAsync();

        // Get bookings relevant to the period
        // If dates provided: check for overlap (Start <= EndRequested AND End >= StartRequested)
        // If no dates: check for active/ongoing (Status == "ACTIVE")
        var bookingsQuery = _context.Bookings
            .Include(b => b.Customer)
            .AsQueryable();

        if (startDate.HasValue && endDate.HasValue)
        {
            // Filter by overlap
            // We want CONFIRMED or ACTIVE bookings covering this period
            bookingsQuery = bookingsQuery.Where(b => 
                (b.BookingStatus == "ACTIVE" || b.BookingStatus == "CONFIRMED") && 
                b.StartDate <= endDate.Value && b.EndDate >= startDate.Value);
        }
        else
        {
            // Default behavior: Current Status (Snapshot of now)
            bookingsQuery = bookingsQuery.Where(b => b.BookingStatus == "ACTIVE");
        }

        var relevantBookings = await bookingsQuery.ToListAsync();

        // Group cars by hub
        var hubGroups = cars.GroupBy(c => c.HubId);
        var hubFleetData = new List<HubFleetData>();

        foreach (var hubGroup in hubGroups)
        {
            var hubCars = hubGroup.ToList();
            var hub = hubCars.First().Hub;

            var carStatusList = new List<CarStatusData>();

            foreach (var car in hubCars)
            {
                // Determine car status based on relevant bookings
                string status;
                RentalInfo? rentalInfo = null;

                var booking = relevantBookings.FirstOrDefault(b => b.CarId == car.CarId);

                if (booking != null)
                {
                    status = "Rented"; // Or "Booked" for future, but keeping "Rented" for UI consistency
                    rentalInfo = new RentalInfo
                    {
                        BookingId = booking.BookingId,
                        CustomerName = $"{booking.Customer?.FirstName} {booking.Customer?.LastName}".Trim(),
                        StartDate = booking.StartDate,
                        EndDate = booking.EndDate,
                        PickupTime = booking.PickupTime
                    };
                }
                else if (car.IsAvailable == "N" || car.IsAvailable == "NO" || car.IsAvailable == "False" || car.IsAvailable == "0")
                {
                     // If searching future dates, maintenance status 'might' not apply unless we track maintenance schedule. 
                     // For now, if no booking found, trust current availability flag ONLY if looking at "now" (no dates)
                     // If looking at future, we assume available unless booked. 
                     // HOWEVER, keeping it simple: If availability flag is strict, mark maintenance.
                     // But strictly speaking, for future query, we only care about bookings.
                     
                     if (!startDate.HasValue) 
                     {
                        status = "Maintenance";
                     }
                     else 
                     {
                         // For date range, if not booked, it's available (ignoring current flag which might be momentary)
                         status = "Available";
                     }
                }
                else
                {
                    status = "Available";
                }

                carStatusList.Add(new CarStatusData
                {
                    CarId = car.CarId,
                    Model = car.CarName ?? "Unknown",
                    CarType = car.CarType?.CarTypeName,
                    RegistrationNumber = car.NumberPlate ?? "N/A",
                    Status = status,
                    DailyRate = car.CarType != null ? (decimal?)car.CarType.DailyRate : null,
                    ImagePath = car.ImagePath,
                    CurrentRental = rentalInfo
                });
            }

            var hubData = new HubFleetData
            {
                HubId = hub?.HubId ?? 0,
                HubName = hub?.HubName ?? "Unknown Hub",
                CityName = hub?.City?.CityName,
                Cars = carStatusList,
                TotalCars = carStatusList.Count,
                AvailableCars = carStatusList.Count(c => c.Status == "Available"),
                RentedCars = carStatusList.Count(c => c.Status == "Rented"),
                MaintenanceCars = carStatusList.Count(c => c.Status == "Maintenance")
            };

            hubFleetData.Add(hubData);
        }

        // Calculate overall statistics
        var totalCars = cars.Count;
        var totalAvailable = hubFleetData.Sum(h => h.AvailableCars);
        var totalRented = hubFleetData.Sum(h => h.RentedCars);
        var totalMaintenance = hubFleetData.Sum(h => h.MaintenanceCars);
        var utilizationRate = totalCars > 0 ? (decimal)totalRented / totalCars * 100 : 0;

        var response = new FleetOverviewResponse
        {
            Hubs = hubFleetData.OrderBy(h => h.HubName).ToList(),
            Statistics = new FleetStatistics
            {
                TotalCars = totalCars,
                TotalAvailable = totalAvailable,
                TotalRented = totalRented,
                TotalMaintenance = totalMaintenance,
                UtilizationRate = Math.Round(utilizationRate, 2)
            }
        };

        return Ok(response);
    }
}

// DTO for staff registration
public class StaffRegistrationRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public HubReference Hub { get; set; }
}

public class HubReference
{
    public int HubId { get; set; }
}
