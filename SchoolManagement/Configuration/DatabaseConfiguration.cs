using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;

namespace SchoolManagement.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services)
    {
        services.AddDbContext<SchoolDbContext>(options =>
            options.UseInMemoryDatabase("SchoolManagementDb"));

        return services;
    }
}
