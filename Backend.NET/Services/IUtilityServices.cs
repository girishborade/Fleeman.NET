using FleetManagementSystem.Api.Models;
using System.Collections.Generic;

namespace FleetManagementSystem.Api.Services;

public interface ISupportService
{
    SupportTicket CreateTicket(SupportTicket ticket);
    List<SupportTicket> GetAllTickets();
    SupportTicket UpdateTicket(int id, SupportTicket ticket);
}

public interface ICheckCustomerExistsService
{
    bool CustomerExists(string email);
}

public interface IGetCarDetailsFromBookingService
{
    CarMaster GetCarDetails(long bookingId);
}

public interface ILocaleService
{
    // Stub for locale logic
    string GetLocaleData(string lang);
}
