namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public interface IPrnCommonBackendService
{
    Task<PaginatedResponse<PrnRawDataDto>> ReadRawPrns(PaginatedRequest request, CancellationToken cancellationToken);
}
