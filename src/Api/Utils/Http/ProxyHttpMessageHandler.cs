using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Defra.LegacyPrns.Api.Utils.Http;

[ExcludeFromCodeCoverage]
public class ProxyHttpMessageHandler : HttpClientHandler
{
    public ProxyHttpMessageHandler(ILogger<ProxyHttpMessageHandler> logger)
    {
        var proxyUri = Environment.GetEnvironmentVariable("HTTP_PROXY");
        var proxy = new WebProxy { BypassProxyOnLocal = true };
        if (proxyUri != null)
        {
            logger.LogDebug("Creating proxy http client");
            proxy.Address = new UriBuilder(proxyUri).Uri;
        }
        else
        {
            logger.LogWarning("HTTP_PROXY is NOT set, proxy client will be disabled");
        }

        Proxy = proxy;
        UseProxy = proxyUri != null;
    }
}
