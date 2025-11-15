using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;
using SchoolManagement.Endpoints;
using SchoolManagement.Repositories;
using SchoolManagement.Services;
using SchoolManagement.Middleware;
using Serilog;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting SchoolManagement API");

    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add global exception handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Add API Versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"),
            new MediaTypeApiVersionReader("version")
        );
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

    // Add services to the container
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "School Management API",
            Version = "v1",
            Description = "RESTful API for managing students and school classes",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "School Management Team"
            }
        });
    });

    // Configure EF Core with In-Memory Database
    builder.Services.AddDbContext<SchoolDbContext>(options =>
        options.UseInMemoryDatabase("SchoolManagementDb"));

    // Register repositories
    builder.Services.AddScoped<IStudentRepository, StudentRepository>();
    builder.Services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();

    // Register services
    builder.Services.AddScoped<IStudentService, StudentService>();
    builder.Services.AddScoped<ISchoolClassService, SchoolClassService>();

    // Add health checks
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "ready" });

    var app = builder.Build();

    // Use global exception handler (must be early in pipeline)
    app.UseExceptionHandler();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API v1");
        });
    }

    app.UseHttpsRedirection();

    // Create API version sets
    var apiVersionSet = app.NewApiVersionSet()
        .HasApiVersion(new ApiVersion(1, 0))
        .ReportApiVersions()
        .Build();

    // Map versioned endpoints
    app.MapStudentEndpoints()
        .WithApiVersionSet(apiVersionSet);
    
    app.MapSchoolClassEndpoints()
        .WithApiVersionSet(apiVersionSet);

    // Map health check endpoints
    app.MapHealthEndpoints();

    // Test endpoints (remove in production)
    if (app.Environment.IsDevelopment())
    {
        app.MapTestEndpoints()
            .WithApiVersionSet(apiVersionSet);
    }

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
