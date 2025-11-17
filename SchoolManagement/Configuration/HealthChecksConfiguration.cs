using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolManagement.Configuration;

public static class HealthChecksConfiguration
{
    public static IServiceCollection AddHealthChecksConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API is running"), tags: new[] { "ready" });

        return services;
    }
}
