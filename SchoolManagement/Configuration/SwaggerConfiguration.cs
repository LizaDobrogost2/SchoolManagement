namespace SchoolManagement.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
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

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API v1");
            });
        }

        return app;
    }
}
