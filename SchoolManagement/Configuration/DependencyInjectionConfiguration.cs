using SchoolManagement.Repositories;
using SchoolManagement.Services;

namespace SchoolManagement.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();

        // Services
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ISchoolClassService, SchoolClassService>();

        return services;
    }
}
