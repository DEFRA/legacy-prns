using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using TestEndpoints = Defra.LegacyPrns.Testing.Endpoints;

namespace Defra.LegacyPrns.Api.Tests.Endpoints.Health;

public class HealthTests(ApiWebApplicationFactory factory, ITestOutputHelper outputHelper)
    : EndpointTestBase(factory, outputHelper)
{
    [Fact]
    public async Task Health_ShouldBeOk()
    {
        var client = CreateClient();

        var response = await client.GetAsync(TestEndpoints.Health.Ready, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthAll_WhenNoDependencies_ShouldBeOkWithNoResults()
    {
        var client = CreateClient();

        var response = await client.GetAsync(TestEndpoints.Health.All(), TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var document = JsonDocument.Parse(content);
        document.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
        document.RootElement.GetProperty("results").EnumerateObject().Should().BeEmpty();
    }
}
