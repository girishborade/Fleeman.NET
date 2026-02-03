using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly HttpClient _httpClient;

    public GoogleAuthService(ApplicationDbContext context, IJwtService jwtService, IUserService userService, HttpClient httpClient)
    {
        _context = context;
        _jwtService = jwtService;
        _userService = userService;
        _httpClient = httpClient;
    }

    public async Task<Dictionary<string, object>> VerifyGoogleTokenAndGetJwtAsync(string accessToken)
    {
        var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");
        
        if (!response.IsSuccessStatusCode)
        {
             throw new ArgumentException("Invalid access token.");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("email", out var emailProp))
        {
            string email = emailProp.GetString();
            
            // 1. Try to find user by Email first (most reliable)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // 2. Generate preferred username
                string preferredUsername = email.Split('@')[0];
                string finalUsername = preferredUsername;

                // 3. Resolve username conflicts
                if (await _context.Users.AnyAsync(u => u.Username == finalUsername))
                {
                    finalUsername = preferredUsername + "_" + Guid.NewGuid().ToString().Substring(0, 4);
                }

                // 4. Create new account via UserService (handles hashing + validation)
                user = new User
                {
                    Email = email,
                    Username = finalUsername,
                    Role = Role.CUSTOMER.ToString(),
                    Password = Guid.NewGuid().ToString() // Will be hashed by AddUser
                };

                var created = _userService.AddUser(user);
                if (created == null)
                {
                    // If AddUser still fails (race condition?), try one last lookup
                    user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email) 
                           ?? throw new Exception("Failed to register or retrieve Google user.");
                }
                else
                {
                    user = created;
                }
            }

            var jwt = _jwtService.GenerateToken(user.Username, user.Role);
            
            // Return full response object to avoid redundant DB lookups in Controller
            var authResponse = new Dictionary<string, object>
            {
                { "token", jwt },
                { "role", user.Role },
                { "userId", user.Id },
                { "email", user.Email },
                { "username", user.Username }
            };

            if (user.HubId.HasValue)
            {
                authResponse["hubId"] = user.HubId;
            }

            // Also check for existing customer ID
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == user.Email);
            if (customer != null)
            {
                authResponse["customerId"] = customer.CustId;
            }

            return authResponse;
        }
        else
        {
             throw new ArgumentException("Email property missing from Google profile.");
        }
    }
}
