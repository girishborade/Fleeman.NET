using System;
using System.Collections.Generic;
using System.Linq;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.DTOs;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;
    private readonly IInvoiceService _invoiceService;
    private readonly ICarService _carService;

    public BookingService(ApplicationDbContext context, IInvoiceService invoiceService, ICarService carService)
    {
        _context = context;
        _invoiceService = invoiceService;
        _carService = carService;
    }

    public BookingResponse CreateBooking(BookingRequest request)
    {
        CarMaster car = null;
        CarTypeMaster carType = null;

        if (request.CarId > 0)
        {
            car = _context.Cars.Include(c => c.CarType).FirstOrDefault(c => c.CarId == request.CarId)
                      ?? throw new ArgumentException("Invalid Car ID");
            carType = car.CarType;
        }
        else if (request.CarTypeId.HasValue && request.CarTypeId.Value > 0)
        {
             // Defer assignment: Just validate CarType exists
             carType = _context.CarTypes.FirstOrDefault(ct => ct.CarTypeId == request.CarTypeId.Value)
                 ?? throw new ArgumentException("Invalid Car Type ID");
        }
        else
        {
            throw new ArgumentException("Invalid Car Selection");
        }

        var customer = _context.Customers.FirstOrDefault(c => c.CustId == request.CustomerId)
                       ?? throw new ArgumentException("Invalid Customer ID");
        var pickupHub = _context.Hubs.FirstOrDefault(h => h.HubId == request.PickupHubId)
                        ?? throw new ArgumentException("Invalid Pickup Hub ID");
        var returnHub = _context.Hubs.FirstOrDefault(h => h.HubId == request.ReturnHubId)
                        ?? throw new ArgumentException("Invalid Return Hub ID");

        var booking = new BookingHeaderTable
        {
            CustomerId = customer.CustId,
            Customer = customer,
            CarId = car?.CarId, // Nullable
            Car = car,          // Nullable
            CarTypeId = carType?.CarTypeId,
            CarType = carType,
            PickupHubId = pickupHub.HubId,
            PickupHub = pickupHub,
            ReturnHubId = returnHub.HubId,
            ReturnHub = returnHub,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            BookingDate = DateTime.Now,
            ConfirmationNumber = "BOK-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
            BookingStatus = "CONFIRMED",
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Address = customer.AddressLine1,
            Pin = customer.Pincode ?? "000000",
            State = customer.City ?? "NA", 
            EmailId = request.Email?.Trim().ToLower(),
            Bookcar = car != null ? ((car.CarName != null && car.CarName.Length > 30) ? car.CarName.Substring(0, 30) : car.CarName) : carType?.CarTypeName
        };

        if (carType != null)
        {
            booking.DailyRate = carType.DailyRate;
            booking.WeeklyRate = carType.WeeklyRate;
            booking.MonthlyRate = carType.MonthlyRate;
        }

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        if (request.AddOnIds != null && request.AddOnIds.Any())
        {
            foreach (var addOnId in request.AddOnIds)
            {
                var addon = _context.AddOns.FirstOrDefault(a => a.AddOnId == addOnId);
                if (addon != null)
                {
                    var detail = new BookingDetailTable
                    {
                        BookingId = booking.BookingId,
                        Booking = booking,
                        AddOnId = addon.AddOnId,
                        AddOn = addon,
                        AddonRate = addon.AddonDailyRate
                    };
                    _context.BookingDetails.Add(detail);
                }
            }
            _context.SaveChanges();
        }

        return MapToResponse(booking);
    }

    public BookingResponse ProcessHandover(HandoverRequest request)
    {
        var booking = _context.Bookings
            .Include(b => b.Car)
            .Include(b => b.Customer)
            .FirstOrDefault(b => b.BookingId == request.BookingId)
            ?? throw new Exception("Booking not found");

        if (!"CONFIRMED".Equals(booking.BookingStatus, StringComparison.OrdinalIgnoreCase))
        {
             throw new Exception("Booking is not in CONFIRMED state");
        }

        if (request.CarId.HasValue)
        {
             var currentCar = booking.Car;
             if (currentCar == null || currentCar.CarId != request.CarId.Value)
             {
                 var newCar = _context.Cars.FirstOrDefault(c => c.CarId == request.CarId.Value)
                     ?? throw new ArgumentException("Invalid Car ID");

                 if (newCar.IsAvailable == "N") // Assuming N means Not Available based on context
                 {
                      // Double check availability logic from Java: isActuallyAvailable()
                      // Java code checked logic. Here simplified.
                      throw new Exception("Selected car is not available");
                 }

                 if (currentCar != null)
                 {
                     currentCar.IsAvailable = "Y";
                 }
                 
                 booking.Car = newCar;
                 booking.Bookcar = newCar.CarName;
                 newCar.IsAvailable = "N";
             }
        }
        else
        {
            if (booking.Car != null)
            {
                booking.Car.IsAvailable = "N";
            }
        }

        booking.PickupTime = DateTime.Now;
        if (request.FuelStatus != null) booking.PickupFuelStatus = request.FuelStatus;
        if (request.Notes != null) booking.PickupCondition = request.Notes;

        booking.BookingStatus = "ACTIVE";
        
        // Handle Date Changes during Handover
        if (request.StartDate.HasValue)
        {
             booking.StartDate = request.StartDate.Value;
        }
        if (request.EndDate.HasValue)
        {
             booking.EndDate = request.EndDate.Value;
        }

        if (booking.StartDate >= booking.EndDate)
        {
            throw new ArgumentException("Start Date must be before End Date");
        }
        
        var invoice = new InvoiceHeaderTable
        {
            BookingId = booking.BookingId,
            Booking = booking,
            CustomerId = booking.CustomerId,
            Customer = booking.Customer,
            CarId = booking.CarId,
            Car = booking.Car,
            HandoverDate = DateTime.Now // DateOnly? using DateTime for now
        };
        _context.Invoices.Add(invoice);
        
        _context.SaveChanges();
        return MapToResponse(booking);
    }

    public BookingResponse ReturnCar(ReturnRequest request)
    {
        if (request == null || request.BookingId <= 0)
        {
            throw new Exception("Invalid Return Request or Booking ID");
        }

        var booking = _context.Bookings
             .Include(b => b.Car)
             .Include(b => b.CarType)
             .Include(b => b.PickupHub)
             .Include(b => b.ReturnHub)
             .FirstOrDefault(b => b.BookingId == request.BookingId)
             ?? throw new Exception($"Booking with ID {request.BookingId} not found");

        string status = booking.BookingStatus?.ToUpper() ?? "";
        if (status != "ACTIVE")
        {
             throw new Exception($"Booking is in {status} state. Only ACTIVE bookings can be returned.");
        }

        booking.BookingStatus = "COMPLETED";
        booking.ReturnTime = DateTime.Now;
        booking.EndDate = request.ReturnDate ?? DateTime.Now;
        if (!string.IsNullOrEmpty(request.FuelStatus)) booking.ReturnFuelStatus = request.FuelStatus;
        if (!string.IsNullOrEmpty(request.Notes)) booking.ReturnCondition = request.Notes;

        if (booking.Car != null)
        {
            booking.Car.IsAvailable = "Y";
        }

        var invoice = _context.Invoices.FirstOrDefault(i => i.BookingId == booking.BookingId);
        if (invoice == null)
        {
            // If invoice was not created during handover for some reason, create it now or warn
            // Based on handover logic, it should exist. 
            // We'll create a minimal one to avoid failure if missing.
            invoice = new InvoiceHeaderTable
            {
                BookingId = booking.BookingId,
                CustomerId = booking.CustomerId,
                CarId = booking.CarId,
                HandoverDate = booking.PickupTime ?? booking.StartDate ?? DateTime.Now.AddDays(-1)
            };
            _context.Invoices.Add(invoice);
        }

        invoice.ReturnDate = request.ReturnDate ?? DateTime.Now;
        
        // Calculate Rates
        DateTime calcStartDate = booking.PickupTime ?? booking.StartDate ?? booking.BookingDate ?? DateTime.Now.AddDays(-1);
        long days = (long)Math.Max(1, (invoice.ReturnDate.Value - calcStartDate).TotalDays);

        double dailyRate = booking.DailyRate ?? 0.0;
        double rentalAmt = days * dailyRate;

        var details = _context.BookingDetails.Include(d => d.AddOn).Where(d => d.BookingId == booking.BookingId).ToList();
        double totalAddonDailyRate = details.Sum(d => d.AddonRate ?? 0);
        double totalAddonAmt = totalAddonDailyRate * days;

        invoice.RentalAmt = rentalAmt;
        invoice.TotalAddonAmt = totalAddonAmt;
        invoice.TotalAmt = rentalAmt + totalAddonAmt;
        invoice.Rate = $"Daily: {dailyRate} | Days: {days}";

        _context.SaveChanges();

        try
        {
            _invoiceService.SendInvoiceEmail(booking.BookingId, booking.EmailId);
        }
        catch (Exception)
        {
            // Ignore email failure
        }

        return MapToResponse(booking);
    }

    public BookingResponse GetBooking(string id)
    {
        if (string.IsNullOrEmpty(id)) throw new Exception("ID cannot be empty");
        
        // Strip '#' prefix if user copied it from UI
        id = id.Trim();
        if (id.StartsWith("#")) id = id.Substring(1).Trim();

        IQueryable<BookingHeaderTable> query = _context.Bookings
            .Include(b => b.Car)
            .Include(b => b.CarType)
            .Include(b => b.Customer)
            .Include(b => b.PickupHub)
            .Include(b => b.ReturnHub);

        BookingHeaderTable booking = null;

        if (long.TryParse(id, out long bookingId))
        {
            booking = query.FirstOrDefault(b => b.BookingId == bookingId);
        }

        if (booking == null)
        {
            booking = query.FirstOrDefault(b => b.ConfirmationNumber == id);
        }

        if (booking == null)
        {
            throw new Exception($"Booking not found with ID or Confirmation Number: {id}");
        }

        return MapToResponse(booking);
    }

    public List<BookingResponse> GetBookingsByEmail(string email)
    {
        return _context.Bookings
             .Include(b => b.Car)
             .Include(b => b.CarType)
             .Include(b => b.Customer)
             .Include(b => b.PickupHub)
             .Include(b => b.ReturnHub)
             .Where(b => b.EmailId == email)
             .ToList() // Materialize first
             .Select(b => MapToResponse(b))
             .ToList();
    }

    public List<BookingResponse> GetAllBookings()
    {
         return _context.Bookings
             .Include(b => b.Car)
             .Include(b => b.CarType)
             .Include(b => b.Customer)
             .Include(b => b.PickupHub)
             .Include(b => b.ReturnHub)
             .ToList() // Materialize first
             .Select(b => MapToResponse(b))
             .ToList();
    }

    public BookingResponse CancelBooking(long bookingId)
    {
        var booking = _context.Bookings.Include(b => b.Car).FirstOrDefault(b => b.BookingId == bookingId)
             ?? throw new Exception("Booking not found");
        
        if ("CANCELLED".Equals(booking.BookingStatus, StringComparison.OrdinalIgnoreCase)) throw new Exception("Already cancelled");
        if ("COMPLETED".Equals(booking.BookingStatus, StringComparison.OrdinalIgnoreCase)) throw new Exception("Cannot cancel completed");

        booking.BookingStatus = "CANCELLED";
        if (booking.Car != null) booking.Car.IsAvailable = "Y";
        
        _context.SaveChanges();
        return MapToResponse(booking);
    }

    public BookingResponse ModifyBooking(long bookingId, BookingRequest request)
    {
         var booking = _context.Bookings.Include(b => b.Car).Include(b => b.CarType).FirstOrDefault(b => b.BookingId == bookingId)
             ?? throw new Exception("Booking not found");

         if (!"CONFIRMED".Equals(booking.BookingStatus, StringComparison.OrdinalIgnoreCase)) throw new Exception("Only CONFIRMED can be modified");

         if (request.StartDate != default) booking.StartDate = request.StartDate;
         if (request.EndDate != default) booking.EndDate = request.EndDate;

         if (request.CarId > 0 && booking.CarId != request.CarId)
         {
              var newCar = _context.Cars.FirstOrDefault(c => c.CarId == request.CarId)
                   ?? throw new ArgumentException("Invalid Car ID");
              
              if (newCar.IsAvailable == "N") throw new Exception("Car unavailable");

              if (booking.Car != null) booking.Car.IsAvailable = "Y";
              
              booking.Car = newCar;
              booking.CarId = newCar.CarId;
              booking.Bookcar = newCar.CarName;
              newCar.IsAvailable = "N"; // Reserve new car
              
              if (booking.CarType != null) booking.DailyRate = booking.CarType.DailyRate;
         }

         _context.SaveChanges();
         return MapToResponse(booking);
    }

    public List<BookingResponse> GetBookingsByHub(int hubId)
    {
        var bookings = _context.Bookings
            .Include(b => b.Car)
            .Include(b => b.CarType)
            .Include(b => b.Customer)
            .Include(b => b.PickupHub)
            .Include(b => b.ReturnHub)
            .Where(b => b.PickupHubId == hubId || b.ReturnHubId == hubId)
            .OrderByDescending(b => b.BookingDate)
            .ToList();

        return bookings.Select(MapToResponse).ToList();
    }

    private BookingResponse MapToResponse(BookingHeaderTable booking)
    {
        // Fail-safe: Ensure Customer is loaded 
        if (booking.Customer == null)
        {
            // Try lookup by ID
            if (booking.CustomerId > 0)
            {
                booking.Customer = _context.Customers.FirstOrDefault(c => c.CustId == booking.CustomerId);
            }
            
            // Try lookup by Email if still null (case-insensitive and trimmed)
            if (booking.Customer == null && !string.IsNullOrEmpty(booking.EmailId))
            {
                string email = booking.EmailId.Trim().ToLower();
                booking.Customer = _context.Customers.FirstOrDefault(c => c.Email != null && c.Email.Trim().ToLower() == email);
            }

            // Still null? Try search by name (case-insensitive and trimmed)
            if (booking.Customer == null && !string.IsNullOrEmpty(booking.FirstName) && !string.IsNullOrEmpty(booking.LastName))
            {
                string fName = booking.FirstName.Trim().ToLower();
                string lName = booking.LastName.Trim().ToLower();
                booking.Customer = _context.Customers.FirstOrDefault(c => 
                    c.FirstName != null && c.FirstName.Trim().ToLower() == fName && 
                    c.LastName != null && c.LastName.Trim().ToLower() == lName);
            }
        }

        // Must fetch details for addons if not already loaded (lazy loading or explicit load)
        // Since we passed entity, EF Core context might be tracking.
        // Safer to query details.
        
        var details = _context.BookingDetails.Include(d => d.AddOn).Where(d => d.BookingId == booking.BookingId).ToList();
        
        long days = 1;
        if (booking.StartDate.HasValue && booking.EndDate.HasValue)
        {
            days = (long)(booking.EndDate.Value - booking.StartDate.Value).TotalDays;
            if (days <= 0) days = 1;
        }
        
        double totalAddonDaily = details.Sum(d => d.AddonRate ?? 0);
        double totalAddonAmt = totalAddonDaily * days;
        double dailyRate = booking.DailyRate ?? 0;
        double totalAmt = (days * dailyRate) + totalAddonAmt;

        // Build detailed add-on list
        var addOnDetails = details
            .Where(d => d.AddOn != null)
            .GroupBy(d => d.AddOnId)
            .Select(g => new AddOnDetailDto
            {
                AddOnId = g.Key ?? 0,
                AddOnName = g.First().AddOn.AddOnName,
                DailyRate = g.First().AddonRate ?? 0,
                Quantity = g.Count() // Count how many times this add-on appears (for child seats)
            })
            .ToList();

        return new BookingResponse
        {
            BookingId = booking.BookingId,
            ConfirmationNumber = booking.ConfirmationNumber,
            BookingStatus = booking.BookingStatus,
            CustomerName = booking.FirstName + " " + booking.LastName,
            Email = booking.EmailId,
            CarName = booking.Bookcar,
            NumberPlate = booking.Car != null ? booking.Car.NumberPlate : null,
            PickupHub = booking.PickupHub != null ? booking.PickupHub.HubName : null,
            PickupHubId = booking.PickupHubId,
            ReturnHub = booking.ReturnHub != null ? booking.ReturnHub.HubName : null,
            CarTypeId = booking.CarTypeId,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            DailyRate = booking.DailyRate,
            TotalAmount = totalAmt,
            TotalAddonAmount = totalAddonAmt,
            SelectedAddOns = details.Where(d => d.AddOn != null).Select(d => d.AddOn.AddOnName).ToList(),
            AddOnDetails = addOnDetails,
            
            // Populate Customer Details with fallbacks
            MobileNumber = (booking.Customer != null ? (booking.Customer.MobileNumber ?? booking.Customer.PhoneNumber) : "") ?? "",
            DrivingLicenseNumber = (booking.Customer != null ? booking.Customer.DrivingLicenseNumber : "") ?? "",
            AddressLine1 = (booking.Customer != null ? booking.Customer.AddressLine1 : booking.Address) ?? "NA",
            City = (booking.Customer != null ? booking.Customer.City : booking.State) ?? "NA", 
            State = booking.State ?? "NA",
            Pincode = (booking.Customer != null ? booking.Customer.Pincode : booking.Pin) ?? "NA"
        };
    }
}
