using System.Net;
using System.Text.Json;
using FluentValidation;
using Serilog; // ✅ NEW

namespace UserManagement.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled Exception Occurred"); // ✅ LOGGING
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;

                response = new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validationException.Errors
                        .Select(e => e.ErrorMessage)
                        .ToList()
                };
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;

                response = new
                {
                    success = false,
                    message = "Unauthorized"
                };
                break;

            case ArgumentException:
                statusCode = (int)HttpStatusCode.BadRequest;

                response = new
                {
                    success = false,
                    message = exception.Message
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;

                response = new
                {
                    success = false,
                    message = "Internal Server Error"
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response);

        await context.Response.WriteAsync(json);
    }
}