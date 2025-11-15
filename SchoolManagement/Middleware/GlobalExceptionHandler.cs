using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SchoolManagement.Middleware;

/// <summary>
/// Global exception handler that provides consistent error responses across the API.
/// Implements IExceptionHandler for .NET 8+ exception handling pipeline.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "An unhandled exception occurred while processing request {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var problemDetails = CreateProblemDetails(httpContext, exception);

        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(exception, statusCode),
            Detail = GetDetail(exception),
            Instance = context.Request.Path,
            Type = GetTypeUrl(statusCode)
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    message = exception.InnerException.Message,
                    type = exception.InnerException.GetType().Name
                };
            }
        }

        return problemDetails;
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string GetTitle(Exception exception, int statusCode)
    {
        return exception switch
        {
            ArgumentNullException => "Bad Request",
            ArgumentException => "Bad Request",
            InvalidOperationException => "Bad Request",
            UnauthorizedAccessException => "Unauthorized",
            NotImplementedException => "Not Implemented",
            TimeoutException => "Request Timeout",
            _ => statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                408 => "Request Timeout",
                409 => "Conflict",
                500 => "Internal Server Error",
                501 => "Not Implemented",
                503 => "Service Unavailable",
                _ => "An error occurred"
            }
        };
    }

    private string GetDetail(Exception exception)
    {
        if (!_environment.IsDevelopment())
        {
            return exception switch
            {
                ArgumentException => "The request contains invalid arguments.",
                InvalidOperationException => "The requested operation is not valid in the current state.",
                UnauthorizedAccessException => "You are not authorized to perform this operation.",
                NotImplementedException => "This feature is not yet implemented.",
                TimeoutException => "The request has timed out.",
                _ => "An unexpected error occurred while processing your request. Please try again later."
            };
        }

        return exception.Message;
    }

    private static string GetTypeUrl(int statusCode)
    {
        return statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            401 => "https://tools.ietf.org/html/rfc7235#section-3.1",
            403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            408 => "https://tools.ietf.org/html/rfc7231#section-6.5.7",
            409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            501 => "https://tools.ietf.org/html/rfc7231#section-6.6.2",
            503 => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
            _ => "https://tools.ietf.org/html/rfc7231"
        };
    }
}
