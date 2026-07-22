using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defra.LegacyPrns.Api.Data.Entities;

public record StatusHistoryLegacy
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid CreatedByOrganisationId { get; init; }

    public int PrnStatusIdFk { get; init; }
}
