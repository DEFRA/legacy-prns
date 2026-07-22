using System.Globalization;
using System.Net;
using AwesomeAssertions;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Models;

namespace Defra.LegacyPrns.Testing.Extensions.WireMock;

public static class PrnCommonBackendExtensions
{
    public static async Task StubPrnCommonBackendRawDataRequest(
        this IWireMockAdminApi wireMock,
        int page,
        int pageSize,
        PaginatedResponse<PrnRawDataDto> response,
        string? accessToken = null
    )
    {
        var builder = wireMock.GetMappingBuilder();

        builder.Given(x =>
            x.WithRequest(r =>
                {
                    r.UsingGet()
                        .WithPath("/api/v1/prn/raw-data")
                        .WithParams(() =>
                            [
                                CreateParam("sourceSystemId", "null"),
                                CreateParam("page", page.ToString(CultureInfo.InvariantCulture)),
                                CreateParam("pageSize", pageSize.ToString(CultureInfo.InvariantCulture)),
                            ]
                        );

                    if (accessToken is not null)
                        r.WithHeader("Authorization", $"Bearer {accessToken}");
                })
                .WithResponse(r => r.WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(response))
        );

        var status = await builder.BuildAndPostAsync(CancellationToken.None);
        status.Guid.Should().NotBeNull();
    }

    public static async Task StubPrnCommonBackendAdminHealth(
        this IWireMockAdminApi wireMock,
        string? accessToken = null
    )
    {
        var builder = wireMock.GetMappingBuilder();

        builder.Given(x =>
            x.WithRequest(r =>
                {
                    r.UsingGet().WithPath("/admin/health");

                    if (accessToken is not null)
                        r.WithHeader("Authorization", $"Bearer {accessToken}");
                })
                .WithResponse(r => r.WithStatusCode(HttpStatusCode.OK))
        );

        var status = await builder.BuildAndPostAsync(CancellationToken.None);
        status.Guid.Should().NotBeNull();
    }

    private static ParamModel CreateParam(string name, string pattern) =>
        new() { Name = name, Matchers = [new MatcherModel { Name = "ExactMatcher", Pattern = pattern }] };
}
