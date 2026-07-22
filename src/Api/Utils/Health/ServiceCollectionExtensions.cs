using System.Diagnostics.CodeAnalysis;
using Defra.LegacyPrns.Api.Data;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.Utils.Health;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHealth(this IServiceCollection services, bool includeDependencyChecks)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        if (includeDependencyChecks)
        {
            healthChecksBuilder.Add(
                new HealthCheckRegistration(
                    MongoDbOptions.SectionName,
                    sp => new MongoHealthCheck(sp.GetRequiredService<IMongoDatabase>()),
                    HealthStatus.Unhealthy,
                    tags: [WebApplicationExtensions.Extended],
                    timeout: TimeSpan.FromSeconds(10)
                )
            );
            healthChecksBuilder.Add(
                new HealthCheckRegistration(
                    PrnCommonBackendOptions.SectionName,
                    sp => new PrnCommonBackendHealthCheck(sp),
                    HealthStatus.Unhealthy,
                    tags: [WebApplicationExtensions.Extended],
                    timeout: TimeSpan.FromSeconds(10)
                )
            );
        }

        return services;
    }
}
