using System.Collections.Generic;
using FleetManagementSystem.Api.DTOs;

namespace FleetManagementSystem.Api.Services;

public interface IBookingService
{
    BookingResponse CreateBooking(BookingRequest request);
    BookingResponse ProcessHandover(HandoverRequest request);
    BookingResponse ReturnCar(ReturnRequest request);
    BookingResponse GetBooking(string bookingId);
    List<BookingResponse> GetBookingsByEmail(string email);
    List<BookingResponse> GetAllBookings();
    List<BookingResponse> GetBookingsByHub(int hubId);
    BookingResponse CancelBooking(long bookingId);
    BookingResponse ModifyBooking(long bookingId, BookingRequest request);
}
