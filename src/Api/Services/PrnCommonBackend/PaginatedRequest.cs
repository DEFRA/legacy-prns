using System.Text.Json.Serialization;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public record PaginatedRequest
{
    [JsonPropertyName("page")]
    public int Page { get; init; } = 1;

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; } = 10;

    [JsonPropertyName("search")]
    public string? Search { get; init; }

    [JsonPropertyName("filterBy")]
    public string? FilterBy { get; init; }

    [JsonPropertyName("sortBy")]
    public string? SortBy { get; init; }
}
