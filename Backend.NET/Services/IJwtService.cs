using System.Security.Claims;
using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Services;

public interface IJwtService
{
    string GenerateToken(string username);
    string GenerateToken(string username, string role);
    string ExtractUserName(string token);
    bool ValidateToken(string token, string username);
}
