using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defra.LegacyPrns.Api.Data.Entities;

public record StatusHistory
{
    public int Id { get; init; }

    public DateTime CreatedOn { get; init; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid CreatedByUser { get; init; }

    public string? PrnStatus { get; init; }

    public string? Comment { get; init; }

    public int? ObligationYear { get; init; }

    public required StatusHistoryLegacy Legacy { get; init; }
}
