using System.Net;
using System.Text.Json;
using FleetManagementSystem.Api.DTOs;

namespace FleetManagementSystem.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var details = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred. Please contact support.";
        if (_env.IsDevelopment() && ex.InnerException != null)
        {
            details += " | Inner Exception: " + ex.InnerException.Message;
        }

        var response = new ErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error",
            Details = details
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(json);
    }
}
