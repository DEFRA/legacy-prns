using System.Diagnostics.CodeAnalysis;

namespace Defra.LegacyPrns.Api.Utils.Http;

[ExcludeFromCodeCoverage]
public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddHttpClientWithTracing<TClient, TImplementation>(
        this IServiceCollection services
    )
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddTransient<ProxyHttpMessageHandler>();

        return services.AddHttpClient<TClient, TImplementation>().AddHeaderPropagation();
    }

    public static IHttpClientBuilder AddHttpClientWithTracingAndProxy<TClient, TImplementation>(
        this IServiceCollection services
    )
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddTransient<ProxyHttpMessageHandler>();

        return services
            .AddHttpClient<TClient, TImplementation>()
            .AddHeaderPropagation()
            .ConfigurePrimaryHttpMessageHandler<ProxyHttpMessageHandler>();
    }
}
