using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.LegacyPrns.Api.Tests.Endpoints;

public class EndpointTestBase : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;

    protected EndpointTestBase(ApiWebApplicationFactory factory, ITestOutputHelper outputHelper)
    {
        _factory = factory;
        _factory.OutputHelper = outputHelper;
    }

    protected virtual void ConfigureTestServices(IServiceCollection services) { }

    protected HttpClient CreateClient()
    {
        var builder = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(ConfigureTestServices);
        });

        return builder.CreateClient();
    }
}
