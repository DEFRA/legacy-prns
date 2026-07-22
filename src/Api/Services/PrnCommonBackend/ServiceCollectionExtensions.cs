using Defra.LegacyPrns.Api.Utils.Http;
using Defra.LegacyPrns.Api.Utils.OAuth2;
using Microsoft.Extensions.Options;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrnCommonBackendService(this IServiceCollection services)
    {
        const string name = PrnCommonBackendOptions.SectionName;

        services.AddOAuth2Client<PrnCommonBackendOptions>(name);
        services
            .AddHttpClientWithTracingAndProxy<IPrnCommonBackendService, PrnCommonBackendService>()
            .AddHttpMessageHandler(sp => sp.GetRequiredKeyedService<OAuth2Handler>(name))
            .ConfigureHttpClient(
                (sp, httpClient) =>
                    sp.GetRequiredService<IOptions<PrnCommonBackendOptions>>().Value.Configure(httpClient)
            );

        return services;
    }
}
