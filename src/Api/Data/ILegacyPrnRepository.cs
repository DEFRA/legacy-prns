using Defra.LegacyPrns.Api.Data.Entities;

namespace Defra.LegacyPrns.Api.Data;

public interface ILegacyPrnRepository
{
    Task DeleteAll(CancellationToken cancellationToken);

    Task InsertMany(IReadOnlyCollection<LegacyPrn> legacyPrns, CancellationToken cancellationToken);
}
