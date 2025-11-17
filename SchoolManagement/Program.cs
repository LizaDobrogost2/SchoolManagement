using SchoolManagement.Configuration;
using SchoolManagement.Middleware;
using Serilog;

try
{
    LoggingConfiguration.ConfigureSerilog();

    Log.Information("Starting SchoolManagement API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddApiVersioningConfiguration();
    builder.Services.AddSwaggerConfiguration();
    builder.Services.AddDatabaseConfiguration();
    builder.Services.AddApplicationServices();

    var app = builder.Build();

    app.ConfigureMiddleware();
    app.UseSwaggerConfiguration();

    var apiVersionSet = app.CreateApiVersionSet();
    app.MapEndpoints(apiVersionSet);

    Log.Information("SchoolManagement API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


// Make Program class accessible for integration tests
public partial class Program { }
