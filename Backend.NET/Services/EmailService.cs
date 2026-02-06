using MailKit.Net.Smtp;
using MimeKit;
using FleetManagementSystem.Api.DTOs;

namespace FleetManagementSystem.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendPasswordResetEmail(string to, string token)
    {
        // Basic Implementation matching Java properties structure
        var host = _configuration["spring:mail:host"] ?? "smtp.gmail.com";
        var port = int.Parse(_configuration["spring:mail:port"] ?? "587");
        var username = _configuration["spring:mail:username"];
        var password = _configuration["spring:mail:password"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // Log warning or just return if not configured
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Fleet Management", username));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = "Password Reset Request";

        message.Body = new TextPart("plain")
        {
            Text = $"Use this token to reset your password: {token}"
        };

        SendEmail(message, host, port, username, password);
    }

    public void SendInvoiceEmailWithAttachment(string to, byte[] pdfBytes, string filename, long bookingId)
    {
        var host = _configuration["spring:mail:host"] ?? "smtp.gmail.com";
        var port = int.Parse(_configuration["spring:mail:port"] ?? "587");
        var username = _configuration["spring:mail:username"];
        var password = _configuration["spring:mail:password"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Email not configured, skipping invoice email");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("IndiaDrive", username));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = $"Your Invoice - Booking #{bookingId}";

        var builder = new BodyBuilder();
        builder.TextBody = $"Dear Customer,\n\nThank you for choosing IndiaDrive!\n\nPlease find attached your invoice for booking #{bookingId}.\n\nBest regards,\nIndiaDrive Team";
        
        // Attach PDF
        builder.Attachments.Add(filename, pdfBytes, new MimeKit.ContentType("application", "pdf"));
        
        message.Body = builder.ToMessageBody();

        SendEmail(message, host, port, username, password);
    }

    public void SendBookingConfirmationEmail(string to, BookingResponse booking)
    {
         var host = _configuration["spring:mail:host"] ?? "smtp.gmail.com";
        var port = int.Parse(_configuration["spring:mail:port"] ?? "587");
        var username = _configuration["spring:mail:username"];
        var password = _configuration["spring:mail:password"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Email not configured, skipping confirmation email");
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("IndiaDrive", username));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = $"Booking Confirmed - #{booking.BookingId}";

        var builder = new BodyBuilder();
        
        string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; color: #333;'>
            <h2 style='color: #2563eb;'>Booking Confirmed!</h2>
            <p>Dear Customer,</p>
            <p>Your booking with IndiaDrive has been successfully confirmed.</p>
            
            <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                <tr style='background-color: #f3f4f6;'>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Booking ID</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>#{booking.BookingId}</td>
                </tr>
                <tr>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Vehicle</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{booking.CarName} ({booking.NumberPlate})</td>
                </tr>
                 <tr style='background-color: #f3f4f6;'>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Pickup</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{booking.PickupHub}<br/>{booking.StartDate:f}</td>
                </tr>
                <tr>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Return</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{booking.ReturnHub}<br/>{booking.EndDate:f}</td>
                </tr>
                <tr style='background-color: #f3f4f6;'>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Total Amount</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd; color: #16a34a; font-weight: bold;'>Rs. {booking.TotalAmount}</td>
                </tr>
            </table>

            <p style='margin-top: 20px;'>You can view your booking details on our website.</p>
            <p>Safe Travels,<br/><strong>IndiaDrive Team</strong></p>
        </div>";

        builder.HtmlBody = htmlBody;
        message.Body = builder.ToMessageBody();

        SendEmail(message, host, port, username, password);
    }

    private void SendEmail(MimeMessage message, string host, int port, string username, string password)
    {
        using (var client = new SmtpClient())
        {
            try 
            {
                client.Connect(host, port, false); // StartTLS usually handles port 587
                client.Authenticate(username, password);
                client.Send(message);
                client.Disconnect(true);
            }
            catch(Exception ex)
            {
                // Simple error handling
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
