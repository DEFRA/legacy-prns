using Defra.LegacyPrns.Api.Data.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.Data;

public class LegacyPrnRepository(IDbContext dbContext) : ILegacyPrnRepository
{
    public async Task DeleteAll(CancellationToken cancellationToken) =>
        await dbContext.LegacyPrns.DeleteManyAsync(FilterDefinition<LegacyPrn>.Empty, cancellationToken);

    public async Task InsertMany(IReadOnlyCollection<LegacyPrn> legacyPrns, CancellationToken cancellationToken)
    {
        if (legacyPrns.Count == 0)
            return;

        var documents = legacyPrns.Select(legacyPrn => legacyPrn with { Id = ObjectId.GenerateNewId() }).ToArray();

        await dbContext.LegacyPrns.InsertManyAsync(documents, cancellationToken: cancellationToken);
    }
}
