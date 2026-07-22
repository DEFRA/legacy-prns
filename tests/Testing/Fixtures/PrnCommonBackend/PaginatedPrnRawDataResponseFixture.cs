using Defra.LegacyPrns.Api.Services.PrnCommonBackend;

namespace Defra.LegacyPrns.Testing.Fixtures.PrnCommonBackend;

public static class PaginatedPrnRawDataResponseFixture
{
    public static PaginatedResponse<PrnRawDataDto> Create(
        int currentPage,
        int pageSize,
        IReadOnlyCollection<PrnRawDataDto> items,
        int totalItems
    ) =>
        new()
        {
            Items = items.ToArray(),
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = totalItems,
        };
}
