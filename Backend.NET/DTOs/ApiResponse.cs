using System;

namespace FleetManagementSystem.Api.DTOs;

public class ApiResponse
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public object Data { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse(string message, bool success, object data)
    {
        Message = message;
        Success = success;
        Data = data;
        Timestamp = DateTime.Now;
    }

    public ApiResponse(string message, object data) : this(message, true, data)
    {
    }
}
