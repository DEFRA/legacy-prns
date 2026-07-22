using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using Defra.LegacyPrns.Testing.Extensions.WireMock;
using TestEndpoints = Defra.LegacyPrns.Testing.Endpoints;

namespace Defra.LegacyPrns.Api.IntegrationTests.Scenarios;

public class HealthTests : IntegrationTestBase
{
    [Fact]
    public async Task HealthAll_WhenDependenciesAreHealthy_ShouldBeOkWithDependencyResults()
    {
        const string bearerValue = "wiremock-prn-common-backend";

        await WireMockContext.WireMockAdminApi.StubTokenRequest(bearerValue, clientId: ClientIds.PrnCommonBackend);
        await WireMockContext.WireMockAdminApi.StubPrnCommonBackendAdminHealth(bearerValue);

        using var client = CreateClient();

        using var response = await client.GetAsync(TestEndpoints.Health.All(), TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var document = JsonDocument.Parse(content);
        document.RootElement.GetProperty("status").GetString().Should().Be("Healthy");
        document
            .RootElement.GetProperty("results")
            .GetProperty("Mongo")
            .GetProperty("status")
            .GetString()
            .Should()
            .Be("Healthy");
        document
            .RootElement.GetProperty("results")
            .GetProperty("PrnCommonBackend")
            .GetProperty("status")
            .GetString()
            .Should()
            .Be("Healthy");
    }
}
