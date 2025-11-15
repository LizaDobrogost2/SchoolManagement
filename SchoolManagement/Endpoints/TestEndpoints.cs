using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.Endpoints;

/// <summary>
/// Test endpoints for demonstrating exception handling and other features.
/// Should be removed in production.
/// </summary>
public static class TestEndpoints
{
    public static RouteGroupBuilder MapTestEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v{version:apiVersion}/test")
            .WithTags("Testing")
            .WithOpenApi();

        group.MapGet("/exception", ThrowException)
            .WithName("ThrowException")
            .WithDescription("Test endpoint that throws an exception to demonstrate global exception handler")
            .Produces<ProblemDetails>(500)
            .MapToApiVersion(1.0);

        group.MapGet("/argument-exception", ThrowArgumentException)
            .WithName("ThrowArgumentException")
            .WithDescription("Test endpoint that throws ArgumentException")
            .Produces<ProblemDetails>(400)
            .MapToApiVersion(1.0);

        group.MapGet("/not-implemented", ThrowNotImplementedException)
            .WithName("ThrowNotImplementedException")
            .WithDescription("Test endpoint that throws NotImplementedException")
            .Produces<ProblemDetails>(501)
            .MapToApiVersion(1.0);

        group.MapGet("/timeout", ThrowTimeoutException)
            .WithName("ThrowTimeoutException")
            .WithDescription("Test endpoint that throws TimeoutException")
            .Produces<ProblemDetails>(408)
            .MapToApiVersion(1.0);

        return group;
    }

    private static IResult ThrowException()
    {
        throw new InvalidOperationException("This is a test exception to demonstrate the global exception handler.");
    }

    private static IResult ThrowArgumentException()
    {
        throw new ArgumentException("Invalid argument provided", "testParameter");
    }

    private static IResult ThrowNotImplementedException()
    {
        throw new NotImplementedException("This feature is not yet implemented.");
    }

    private static IResult ThrowTimeoutException()
    {
        throw new TimeoutException("The operation has timed out.");
    }
}
