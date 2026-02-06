using System.Linq;
using System.Collections.Generic;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Services;

public class SupportService : ISupportService
{
    private readonly ApplicationDbContext _context;
    public SupportService(ApplicationDbContext context) => _context = context;
    public SupportTicket CreateTicket(SupportTicket ticket)
    {
        _context.SupportTickets.Add(ticket);
        _context.SaveChanges();
        return ticket;
    }
    public List<SupportTicket> GetAllTickets() => _context.SupportTickets.ToList();
    public SupportTicket UpdateTicket(int id, SupportTicket ticket)
    {
         var existing = _context.SupportTickets.Find(id);
         if (existing != null)
         {
              // Update all fields or specific ones
              existing.Status = ticket.Status; // Example
              // ... map others
              _context.SaveChanges();
              return existing;
         }
         return null;
    }
}

public class CheckCustomerExistsService : ICheckCustomerExistsService
{
    private readonly ApplicationDbContext _context;
    public CheckCustomerExistsService(ApplicationDbContext context) => _context = context;

    public bool CustomerExists(string email)
    {
        return _context.Customers.Any(c => c.Email == email);
    }
}

public class GetCarDetailsFromBookingService : IGetCarDetailsFromBookingService
{
    private readonly ApplicationDbContext _context;
    public GetCarDetailsFromBookingService(ApplicationDbContext context) => _context = context;

    public CarMaster GetCarDetails(long bookingId)
    {
        var booking = _context.Bookings.Include(b => b.Car).FirstOrDefault(b => b.BookingId == bookingId);
        return booking?.Car;
    }
}

public class LocaleService : ILocaleService
{
    public string GetLocaleData(string lang)
    {
        // Return dummy json or similar, or just stub
        return "{}";
    }
}
