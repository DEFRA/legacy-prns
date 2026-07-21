using System.Text.Json.Serialization;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public record PaginatedResponse<T>
{
    [JsonPropertyName("items")]
    public T[]? Items { get; init; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; init; }

    [JsonPropertyName("totalItems")]
    public int TotalItems { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("searchTerm")]
    public string? SearchTerm { get; init; }

    [JsonPropertyName("filterBy")]
    public string? FilterBy { get; init; }

    [JsonPropertyName("sortBy")]
    public string? SortBy { get; init; }

    [JsonPropertyName("typeAhead")]
    public string[]? TypeAhead { get; init; }

    [JsonPropertyName("pageCount")]
    public int PageCount => PageSize == 0 ? 0 : (TotalItems + (PageSize - 1)) / PageSize;
}
