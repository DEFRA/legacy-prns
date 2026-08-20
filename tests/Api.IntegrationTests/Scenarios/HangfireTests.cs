using System.Net;
using System.Net.Http.Headers;
using AwesomeAssertions;
using TestEndpoints = Defra.LegacyPrns.Testing.Endpoints;

namespace Defra.LegacyPrns.Api.IntegrationTests.Scenarios;

public class HangfireTests : IntegrationTestBase
{
    [Fact]
    public async Task Dashboard_WhenNotAuthenticated_ShouldBeUnauthorized()
    {
        using var client = CreateClient();

        using var response = await client.GetAsync(
            TestEndpoints.Hangfire.Dashboard,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.WwwAuthenticate.Should().Contain(x => x.Scheme == "Basic");
    }

    [Fact]
    public async Task Dashboard_WhenAuthenticated_ShouldBeOk()
    {
        using var client = CreateAuthenticatedClient();

        using var response = await client.GetAsync(
            TestEndpoints.Hangfire.Dashboard,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RecurringJobs_WhenRegistered_ShouldIncludeMigrateLegacyPrns()
    {
        using var client = CreateAuthenticatedClient();

        using var response = await client.GetAsync(
            TestEndpoints.Hangfire.RecurringJobs(),
            TestContext.Current.CancellationToken
        );
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("MigrateLegacyPrns");
    }

    private static HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        var credentials = Convert.ToBase64String("developer:password"u8);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        return client;
    }
}
