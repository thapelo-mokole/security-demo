using System.Net;
using System.Text.Json;

namespace SecureApp.Middleware;

// SECURE: Centralized exception handling to prevent information disclosure
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // SECURE: Log detailed error for monitoring but return generic message to client
            _logger.LogError(ex, "An unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            Message = "An error occurred while processing your request.",
            TraceId = context.TraceIdentifier
        };

        // SECURE: Don't expose internal error details in production
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}