using Microsoft.AspNetCore.Mvc;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.Services;
using FleetManagementSystem.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly ApplicationDbContext _context; // Replacing CustomerRepository
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IEmailService _emailService;

    public UserController(IUserService userService, IJwtService jwtService, ApplicationDbContext context, IGoogleAuthService googleAuthService, IEmailService emailService)
    {
        _userService = userService;
        _jwtService = jwtService;
        _context = context;
        _googleAuthService = googleAuthService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Password = request.Password,
            Email = request.Email,
            Role = Role.CUSTOMER.ToString()
        };

        var createdUser = _userService.AddUser(user);
        if (createdUser == null)
        {
            return BadRequest("User creation failed. Username or Email might already exist.");
        }
        return Created("", createdUser); 
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        try
        {
            if (_userService.ValidateCredentials(loginRequest.Username, loginRequest.Password))
            {
                // Authenticated
                 var jwt = _jwtService.GenerateToken(loginRequest.Username);
                 var fullUser = _userService.GetUserByUsername(loginRequest.Username);

                 var response = new Dictionary<string, object>
                 {
                     { "token", jwt },
                     { "role", fullUser.Role },
                     { "userId", fullUser.Id },
                     { "email", fullUser.Email },
                     { "username", fullUser.Username }
                 };

                 if (fullUser.HubId.HasValue)
                 {
                     response["hubId"] = fullUser.HubId;
                 }

                 var customer = _context.Customers.FirstOrDefault(c => c.Email == fullUser.Email);
                 if (customer != null)
                 {
                     response["customerId"] = customer.CustId;
                 }
                 else
                 {
                     response["customerId"] = fullUser.Id;
                 }

                 return Ok(response);
            }
            else
            {
                return Unauthorized("Authentication failed");
            }
        }
        catch (Exception ex)
        {
            return Unauthorized("Invalid Creds");
        }
    }

    [HttpPost("api/v1/auth/google")]
    public async Task<IActionResult> GoogleLogin([FromBody] Dictionary<string, string> request)
    {
        try
        {
            if (request.TryGetValue("token", out var token))
            {
                var authResponse = await _googleAuthService.VerifyGoogleTokenAndGetJwtAsync(token);
                return Ok(authResponse);
            }
             return Unauthorized("Invalid Google Token");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal Server Error: " + ex.Message + " | " + ex.InnerException?.Message);
        }
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] Dictionary<string, string> request)
    {
        if (request.TryGetValue("email", out var email))
        {
            var token = _userService.GenerateResetToken(email);
            if (token != null)
            {
                _emailService.SendPasswordResetEmail(email, token);
            }
        }
        return Ok("If an account exists with this email, you will receive a reset link shortly.");
    }

    [HttpPost("reset-password")]
    public IActionResult ResetPassword([FromBody] Dictionary<string, string> request)
    {
        var token = request.TryGetValue("token", out var t) ? t : null;
        var password = request.TryGetValue("password", out var p) ? p : null;

        if (token != null && password != null && _userService.ResetPassword(token, password))
        {
            return Ok("Password has been reset successfully.");
        }
        return BadRequest("Invalid or expired reset token.");
    }
}
