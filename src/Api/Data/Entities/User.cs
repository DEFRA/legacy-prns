using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defra.LegacyPrns.Api.Data.Entities;

public record User
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid? Id { get; init; }

    public string? Name { get; init; }
}
