using System;
using System.Collections.Generic;
using System.Linq;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Services;

public class CarService : ICarService
{
    private readonly ApplicationDbContext _context;

    public CarService(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<object[]> GetCarsByHubAddress(string hubAddress)
    {
        // Custom query logic parity
        // In Java: carRepository.findCarDetailsByHubAddress(hub_address_and_details)
        // Assuming it joins Hub, Car, CarType.
        // Returning List<Object[]> suggests partial data.
        // Let's implement a reasonable join returning data matching frontend expectation.
        
        var query = from car in _context.Cars
                    join hub in _context.Hubs on car.HubId equals hub.HubId
                    join carType in _context.CarTypes on car.CarTypeId equals carType.CarTypeId
                    where hub.HubAddressAndDetails == hubAddress
                    select new object[]
                    {
                        car.CarId,
                        car.CarName,
                        car.NumberPlate,
                        car.Status,
                        car.Mileage,
                        carType.CarTypeName,
                        carType.DailyRate,
                        hub.HubName
                    };
        
        return query.ToList();
    }

    public List<CarMaster> GetAvailableCars(int hubId, DateTime startDate, DateTime endDate, long? carTypeId)
    {
        // Logic: Find cars at the hub that are NOT overlapping with confirmed/active bookings in the date range.
        
        var bookedCarIds = _context.Bookings
            .Where(b => b.PickupHubId == hubId &&
                        (b.BookingStatus == "CONFIRMED" || b.BookingStatus == "ACTIVE") &&
                        ((b.StartDate <= endDate && b.EndDate >= startDate))) // Overlap check
            .Select(b => b.CarId)
            .Where(id => id.HasValue)
            .Select(id => id.Value)
            .Distinct();

        var query = _context.Cars
            .Include(c => c.CarType)
            .AsQueryable();

        query = query.Where(c => c.HubId == hubId && (c.IsAvailable == "Y" || c.IsAvailable == "YES"));

        if (carTypeId.HasValue)
        {
            query = query.Where(c => c.CarTypeId == carTypeId.Value);
        }

        query = query.Where(c => !bookedCarIds.Contains(c.CarId));

        return query.ToList();
    }
}
