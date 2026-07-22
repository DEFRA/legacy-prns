using Defra.LegacyPrns.Api.Data.Entities;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.Data;

public class MongoDbContext(IMongoDatabase database) : IDbContext
{
    public IMongoCollection<LegacyPrn> LegacyPrns { get; } = database.GetCollection<LegacyPrn>(nameof(LegacyPrn));
}
