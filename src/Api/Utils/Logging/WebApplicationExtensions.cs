using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace Defra.LegacyPrns.Api.Utils.Logging;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static WebApplication UseRequestLogging(this WebApplication app, bool integrationTest)
    {
        if (integrationTest)
            return app;

        app.UseSerilogRequestLogging();

        return app;
    }
}
