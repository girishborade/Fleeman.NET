using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.DTOs;
using FleetManagementSystem.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementSystem.Api.Controllers;

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
    public async Task<ActionResult<FleetOverviewResponse>> GetFleetOverview()
    {
        // Get all cars with hub and booking information
        var cars = await _context.Cars
            .Include(c => c.Hub)
                .ThenInclude(h => h!.City)
            .Include(c => c.CarType)
            .ToListAsync();

        // Get all active bookings
        var activeBookings = await _context.Bookings
            .Include(b => b.Customer)
            .Where(b => b.BookingStatus == "ACTIVE")
            .ToListAsync();

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
                // Determine car status
                string status;
                RentalInfo? rentalInfo = null;

                var activeBooking = activeBookings.FirstOrDefault(b => b.CarId == car.CarId);

                if (activeBooking != null)
                {
                    status = "Rented";
                    rentalInfo = new RentalInfo
                    {
                        BookingId = activeBooking.BookingId,
                        CustomerName = $"{activeBooking.Customer?.FirstName} {activeBooking.Customer?.LastName}".Trim(),
                        StartDate = activeBooking.StartDate,
                        EndDate = activeBooking.EndDate,
                        PickupTime = activeBooking.PickupTime
                    };
                }
                else if (car.IsAvailable == "N" || car.IsAvailable == "NO" || car.IsAvailable == "False" || car.IsAvailable == "0")
                {
                    status = "Maintenance";
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
