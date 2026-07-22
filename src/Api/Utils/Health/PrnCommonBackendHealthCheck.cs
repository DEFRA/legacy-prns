using System.Diagnostics.CodeAnalysis;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using Defra.LegacyPrns.Api.Utils.Http;
using Defra.LegacyPrns.Api.Utils.OAuth2;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Defra.LegacyPrns.Api.Utils.Health;

[ExcludeFromCodeCoverage]
public class PrnCommonBackendHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var proxyHandler = serviceProvider.GetRequiredService<ProxyHttpMessageHandler>();
            var oAuth2Handler = serviceProvider.GetRequiredKeyedService<OAuth2Handler>(
                PrnCommonBackendOptions.SectionName
            );

            oAuth2Handler.InnerHandler = proxyHandler;

            using var httpClient = new HttpClient(oAuth2Handler);

            serviceProvider.GetRequiredService<IOptions<PrnCommonBackendOptions>>().Value.Configure(httpClient);

            const string health = "admin/health";
            using var response = await httpClient.GetAsync(health, cancellationToken);

            response.EnsureSuccessStatusCode();

            return HealthCheckResult.Healthy($"Connected to {httpClient.BaseAddress}{health}");
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                exception: new Exception($"Failed to connect to {PrnCommonBackendOptions.SectionName}", exception)
            );
        }
    }
}
