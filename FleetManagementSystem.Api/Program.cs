using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IUserService, FleetManagementSystem.Api.Services.UserService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IJwtService, FleetManagementSystem.Api.Services.JwtService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IEmailService, FleetManagementSystem.Api.Services.EmailService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IGoogleAuthService, FleetManagementSystem.Api.Services.GoogleAuthService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ICarService, FleetManagementSystem.Api.Services.CarService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IInvoiceService, FleetManagementSystem.Api.Services.InvoiceService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IBookingService, FleetManagementSystem.Api.Services.BookingService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ICarTypeMasterService, FleetManagementSystem.Api.Services.CarTypeMasterService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IExcelUploadService, FleetManagementSystem.Api.Services.ExcelUploadService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IStateService, FleetManagementSystem.Api.Services.StateService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ICityService, FleetManagementSystem.Api.Services.CityService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IHubService, FleetManagementSystem.Api.Services.HubService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IAirportService, FleetManagementSystem.Api.Services.AirportService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IAddOnService, FleetManagementSystem.Api.Services.AddOnService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IVendorService, FleetManagementSystem.Api.Services.VendorService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ICustomerService, FleetManagementSystem.Api.Services.CustomerService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ISupportService, FleetManagementSystem.Api.Services.SupportService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ICheckCustomerExistsService, FleetManagementSystem.Api.Services.CheckCustomerExistsService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.IGetCarDetailsFromBookingService, FleetManagementSystem.Api.Services.GetCarDetailsFromBookingService>();
builder.Services.AddScoped<FleetManagementSystem.Api.Services.ILocaleService, FleetManagementSystem.Api.Services.LocaleService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}
builder.Services.AddDbContext<FleetManagementSystem.Api.Data.ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23))));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseMiddleware<FleetManagementSystem.Api.Middleware.ExceptionMiddleware>();

app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
