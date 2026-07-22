using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public class PrnCommonBackendService(HttpClient httpClient) : IPrnCommonBackendService
{
    private const string RawDataPath = "api/v1/prn/raw-data";
    private const string NullSourceSystemId = "null";

    public async Task<PaginatedResponse<PrnRawDataDto>> ReadRawPrns(
        PaginatedRequest request,
        CancellationToken cancellationToken
    )
    {
        using var response = await httpClient.GetAsync(CreateRawDataRequestUri(request), cancellationToken);

        response.EnsureSuccessStatusCode();

        var prns =
            await response.Content.ReadFromJsonAsync<PaginatedResponse<PrnRawDataDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Empty PRN raw-data response.");

        return prns;
    }

    private static string CreateRawDataRequestUri(PaginatedRequest request)
    {
        var query = new Dictionary<string, string?>
        {
            ["sourceSystemId"] = NullSourceSystemId,
            ["page"] = request.Page.ToString(CultureInfo.InvariantCulture),
            ["pageSize"] = request.PageSize.ToString(CultureInfo.InvariantCulture),
        };

        AddIfPresent(query, "search", request.Search);
        AddIfPresent(query, "filterBy", request.FilterBy);
        AddIfPresent(query, "sortBy", request.SortBy);

        return QueryHelpers.AddQueryString(RawDataPath, query);
    }

    private static void AddIfPresent(Dictionary<string, string?> query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            query[key] = value;
    }
}
