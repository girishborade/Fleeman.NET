using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace FleetManagementSystem.Api.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public User AddUser(User user)
    {
        // Check if user already exists to prevent duplicate entry errors
        if (_context.Users.Any(u => u.Email == user.Email || u.Username == user.Username))
        {
            return null; // Controller will handle this as a failure
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User GetUserByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    public string GenerateResetToken(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.Now.AddHours(1);
            _context.SaveChanges();
            return token;
        }
        return null;
    }

    public bool ResetPassword(string token, string newPassword)
    {
        var user = _context.Users.FirstOrDefault(u => u.ResetToken == token);
        if (user != null)
        {
            if (user.ResetTokenExpiry.HasValue && user.ResetTokenExpiry.Value > DateTime.Now)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.ResetToken = null;
                user.ResetTokenExpiry = null;
                _context.SaveChanges();
                return true;
            }
        }
        return false;
    }

    public bool ValidateCredentials(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            try 
            {
                Console.WriteLine($"[Auth] Verifying user: {username}");
                Console.WriteLine($"[Auth] Stored Hash: {user.Password}");
                bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.Password);
                Console.WriteLine($"[Auth] Match Result: {isMatch}");
                if (isMatch) return true;
            }
            catch (Exception)
            {
                // Verify failed or invalid hash format, fall through to plain text check
            }

            // Fallback: Check if stored password is plain text (for seed data)
            if (user.Password == password) return true;
        }
        return false;
    }
}
