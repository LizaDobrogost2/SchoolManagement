using Serilog;

namespace SchoolManagement.Configuration;

public static class MiddlewareConfiguration
{
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });

        app.UseHttpsRedirection();

        return app;
    }
}
