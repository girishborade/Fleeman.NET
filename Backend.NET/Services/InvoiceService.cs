using System;
using System.Linq;
using FleetManagementSystem.Api.Data;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FleetManagementSystem.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IEmailService _emailService;
    private readonly ApplicationDbContext _context;

    public InvoiceService(IEmailService emailService, ApplicationDbContext context)
    {
        _emailService = emailService;
        _context = context;
    }

    public void SendInvoiceEmail(long bookingId, string email)
    {
        try
        {
            // Fetch booking with all related data
            var booking = _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.CarType)
                .Include(b => b.Customer)
                .Include(b => b.PickupHub)
                .Include(b => b.ReturnHub)
                .FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                Console.WriteLine($"Booking {bookingId} not found");
                return;
            }

            // Fetch add-on details
            var details = _context.BookingDetails
                .Include(d => d.AddOn)
                .Where(d => d.BookingId == bookingId)
                .ToList();

            // Fetch invoice if exists
            var invoice = _context.Invoices.FirstOrDefault(i => i.BookingId == bookingId);

            // Calculate days
            DateTime startDate = booking.PickupTime ?? booking.StartDate ?? booking.BookingDate ?? DateTime.Now;
            DateTime endDate = invoice?.ReturnDate ?? booking.EndDate ?? DateTime.Now;
            long days = Math.Max(1, (long)(endDate - startDate).TotalDays);

            // Calculate amounts
            double dailyRate = booking.DailyRate ?? 0;
            double baseRental = dailyRate * days;
            
            var addOnGroups = details
                .Where(d => d.AddOn != null)
                .GroupBy(d => d.AddOnId)
                .Select(g => new
                {
                    Name = g.First().AddOn.AddOnName,
                    DailyRate = g.First().AddonRate ?? 0,
                    Quantity = g.Count()
                })
                .ToList();

            double totalAddonDaily = details.Sum(d => d.AddonRate ?? 0);
            double totalAddon = totalAddonDaily * days;
            double grandTotal = baseRental + totalAddon;

            // Generate PDF
            QuestPDF.Settings.License = LicenseType.Community; // Use Community license
            
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(content => ComposeContent(content, booking, details, addOnGroups, days, dailyRate, baseRental, totalAddon, grandTotal, startDate, endDate));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            }).GeneratePdf();

            // Send email with PDF attachment
            _emailService.SendInvoiceEmailWithAttachment(email, pdfBytes, $"Invoice_{bookingId}.pdf", bookingId);
            
            Console.WriteLine($"Invoice sent successfully for booking {bookingId} to {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending invoice: {ex.Message}");
        }
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("IndiaDrive").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                column.Item().Text("Premium Fleet Management").FontSize(12).FontColor(Colors.Grey.Darken1);
            });

            row.RelativeItem().Column(column =>
            {
                column.Item().AlignRight().Text("INVOICE").FontSize(20).Bold();
                column.Item().AlignRight().Text($"Date: {DateTime.Now:dd MMM yyyy}").FontSize(10);
            });
        });
    }

    private void ComposeContent(IContainer container, dynamic booking, dynamic details, dynamic addOnGroups, 
        long days, double dailyRate, double baseRental, double totalAddon, double grandTotal, DateTime startDate, DateTime endDate)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(15);

            // Booking Info
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Booking Information").Bold().FontSize(14);
                    col.Item().Text($"Booking ID: {booking.BookingId}");
                    col.Item().Text($"Confirmation: {booking.ConfirmationNumber}");
                    col.Item().Text($"Status: {booking.BookingStatus}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Customer Information").Bold().FontSize(14);
                    col.Item().Text($"{booking.FirstName} {booking.LastName}");
                    col.Item().Text($"{booking.EmailId}");
                    col.Item().Text($"{booking.Address}");
                });
            });

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            // Rental Period
            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"Pickup: {startDate:dd MMM yyyy}");
                row.RelativeItem().Text($"Return: {endDate:dd MMM yyyy}");
                row.RelativeItem().Text($"Duration: {days} day{(days > 1 ? "s" : "")}").Bold();
            });

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            // Cost Breakdown Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Description").Bold();
                    header.Cell().Element(CellStyle).Text("Rate").Bold();
                    header.Cell().Element(CellStyle).Text("Qty").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Amount").Bold();
                });

                // Vehicle Rental
                table.Cell().Element(CellStyle).Column(col =>
                {
                    col.Item().Text("Vehicle Rental").Bold();
                    col.Item().Text((string)(booking.Bookcar ?? "Premium Vehicle")).FontSize(10).FontColor(Colors.Grey.Darken1);
                });
                table.Cell().Element(CellStyle).Text($"₹{dailyRate:N0}/day");
                table.Cell().Element(CellStyle).Text($"{days}");
                table.Cell().Element(CellStyle).AlignRight().Text($"₹{baseRental:N0}");

                // Add-ons
                foreach (var addon in addOnGroups)
                {
                    var effectiveDailyRate = addon.DailyRate * addon.Quantity;
                    var addonTotal = effectiveDailyRate * days;
                    var displayName = addon.Quantity > 1 ? $"{addon.Name} (x{addon.Quantity})" : addon.Name;

                    table.Cell().Element(CellStyle).Column(col =>
                    {
                        col.Item().Text("Add-on Service").Bold();
                        col.Item().Text((string)displayName).FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                    table.Cell().Element(CellStyle).Text($"₹{effectiveDailyRate:N0}/day");
                    table.Cell().Element(CellStyle).Text($"{days}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"₹{addonTotal:N0}");
                }

                // Totals
                table.Cell().ColumnSpan(3).Element(CellStyle).AlignRight().Text("Subtotal:").Bold();
                table.Cell().Element(CellStyle).AlignRight().Text($"₹{(baseRental + totalAddon):N0}").Bold();

                table.Cell().ColumnSpan(3).Element(CellStyle).AlignRight().Text("TOTAL AMOUNT:").Bold().FontSize(14);
                table.Cell().Element(CellStyle).AlignRight().Text($"₹{grandTotal:N0}").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
            });

            column.Item().PaddingTop(20).Text("Thank you for choosing IndiaDrive!").FontSize(12).Italic();
        });
    }

    private IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
    }
}
