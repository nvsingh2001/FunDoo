namespace ModelLayer.Utilities;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string[] Errors { get; set; }

    // Success Response Constructor
    public ApiResponse(bool success, string message, T data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    // Error Response Constructor
    public ApiResponse(bool success, string message, params string[]? errors)
    {
        Success = success;
        Message = message;
        Errors = errors;
    }

    // Parameterless Constructor
    public ApiResponse() { }
}
