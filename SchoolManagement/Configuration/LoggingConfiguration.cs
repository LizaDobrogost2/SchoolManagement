using Serilog;

namespace SchoolManagement.Configuration;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build())
            .CreateLogger();
    }
}
