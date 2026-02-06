using Microsoft.EntityFrameworkCore;
using FleetManagementSystem.Api.Models;

namespace FleetManagementSystem.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    // Roles is an Enum, not a table
    public DbSet<StateMaster> States { get; set; }
    public DbSet<CityMaster> Cities { get; set; }
    public DbSet<HubMaster> Hubs { get; set; }
    public DbSet<AddOnMaster> AddOns { get; set; }
    public DbSet<AirportMaster> Airports { get; set; }
    public DbSet<CarTypeMaster> CarTypes { get; set; }
    public DbSet<CarMaster> Cars { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<CustomerMaster> Customers { get; set; }
    public DbSet<BookingHeaderTable> Bookings { get; set; }
    public DbSet<BookingDetailTable> BookingDetails { get; set; }
    public DbSet<InvoiceHeaderTable> Invoices { get; set; }
    public DbSet<InvoiceDetailTable> InvoiceDetails { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map Role Enum to String
        // modelBuilder.Entity<User>()
        //     .Property(u => u.Role)
        //     .HasConversion(
        //         v => v.ToString(),
        //         v => (Role)Enum.Parse(typeof(Role), v));
    }
}
