namespace FleetManagementSystem.Api.Services;

public interface IEmailService
{
    void SendPasswordResetEmail(string to, string token);
    void SendInvoiceEmailWithAttachment(string to, byte[] pdfBytes, string filename, long bookingId);
    void SendBookingConfirmationEmail(string to, DTOs.BookingResponse booking);
}
