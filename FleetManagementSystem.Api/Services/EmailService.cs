using MailKit.Net.Smtp;
using MimeKit;

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
