using Asp.Versioning;
using Asp.Versioning.Builder;
using SchoolManagement.Endpoints;

namespace SchoolManagement.Configuration;

public static class EndpointsConfiguration
{
    public static WebApplication MapEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        app.MapStudentEndpoints()
            .WithApiVersionSet(apiVersionSet);

        app.MapSchoolClassEndpoints()
            .WithApiVersionSet(apiVersionSet);

        app.MapHealthEndpoints();

        return app;
    }
}
