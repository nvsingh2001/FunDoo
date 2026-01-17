using System.Text.Json;
using ModelLayer.Utilities;

namespace FunDooApp.Middleware;
public class GlobalExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, 
                "An unhandled exception has occurred: {Message}", 
                exception.Message);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiResponse<object> response;
        
        switch (exception)
        {
            case KeyNotFoundException knfEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ApiResponse<object>(false, knfEx.Message);
                break;

            case InvalidOperationException ioEx:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new ApiResponse<object>(false, ioEx.Message);
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ApiResponse<object>(false, argEx.Message);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ApiResponse<object>(false, 
                    "Internal server error. Please try again later.");
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(jsonResponse);
    }
}
