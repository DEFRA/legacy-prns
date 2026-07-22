using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defra.LegacyPrns.Api.Data.Entities;

public record Organisation
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; init; }

    public required string Name { get; init; }
}
