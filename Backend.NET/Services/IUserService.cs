using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Services;

public interface IUserService
{
    User AddUser(User user);
    User GetUserByUsername(string username);
    string GenerateResetToken(string email);
    bool ResetPassword(string token, string newPassword);
    bool ValidateCredentials(string username, string password);
}
