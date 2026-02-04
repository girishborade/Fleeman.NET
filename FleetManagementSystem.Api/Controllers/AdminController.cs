using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
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
