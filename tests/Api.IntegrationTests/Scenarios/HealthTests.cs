using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using TestEndpoints = Defra.LegacyPrns.Testing.Endpoints;

namespace Defra.LegacyPrns.Api.IntegrationTests.Scenarios;

public class HealthTests : IntegrationTestBase
{
    [Fact]
    public async Task HealthAll_WhenMongoHealthy_ShouldBeOkWithMongoResult()
    {
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
    }
}
