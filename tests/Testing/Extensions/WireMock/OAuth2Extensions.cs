using System.Net;
using AwesomeAssertions;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Models;

namespace Defra.LegacyPrns.Testing.Extensions.WireMock;

public static class OAuth2Extensions
{
    public const string AccessToken = "access_token";

    public static async Task StubTokenRequest(
        this IWireMockAdminApi wireMock,
        string accessToken = AccessToken,
        int expiryInSeconds = 3600,
        string? scope = "scope",
        string clientId = "client_id"
    )
    {
        var builder = wireMock.GetMappingBuilder();
        object[] patterns = ["grant_type=client_credentials", $"client_id={clientId}", "client_secret=client_secret"];

        if (scope is not null)
            patterns = [.. patterns, $"scope={scope}"];

        builder.Given(x =>
            x.WithRequest(r =>
                    r.UsingPost()
                        .WithPath("/oauth2/v2.0/token")
                        .WithHeader("Content-Type", "application/x-www-form-urlencoded")
                        .WithBody(() =>
                            new BodyModel
                            {
                                Matchers =
                                [
                                    .. patterns.Select(pattern => new MatcherModel
                                    {
                                        Name = "FormUrlEncodedMatcher",
                                        Pattern = pattern,
                                        MatchOperator = "And",
                                    }),
                                ],
                                MatchOperator = "And",
                            }
                        )
                )
                .WithResponse(r =>
                    r.WithStatusCode(HttpStatusCode.OK)
                        .WithBodyAsJson(new { access_token = accessToken, expires_in = expiryInSeconds })
                )
        );

        var status = await builder.BuildAndPostAsync(CancellationToken.None);
        status.Guid.Should().NotBeNull();
    }
}
