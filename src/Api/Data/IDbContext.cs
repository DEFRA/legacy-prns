using Defra.LegacyPrns.Api.Data.Entities;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.Data;

public interface IDbContext
{
    IMongoCollection<LegacyPrn> LegacyPrns { get; }
}
