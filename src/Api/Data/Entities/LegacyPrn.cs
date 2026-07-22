using MongoDB.Bson;

namespace Defra.LegacyPrns.Api.Data.Entities;

public record LegacyPrn
{
    public ObjectId Id { get; init; }

    public string SchemaVersion { get; init; } = "v1.0";

    public int Version { get; init; }

    public required string PrnNumber { get; init; }

    public required Organisation Organisation { get; init; }

    public required string ProducerAgency { get; init; }

    public required string ReprocessorExporterAgency { get; init; }

    public int TonnageValue { get; init; }

    public required string MaterialName { get; init; }

    public string? Notes { get; init; }

    public string? PrnSignatory { get; init; }

    public string? PrnSignatoryPosition { get; init; }

    public required string IssuedByOrg { get; init; }

    public string? Signature { get; init; }

    public DateTime IssueDate { get; init; }

    public bool IsDecemberWaste { get; init; }

    public required string AccreditationNumber { get; init; }

    public string? ReprocessingSite { get; init; }

    public int? AccreditationYear { get; init; }

    public int? ObligationYear { get; init; }

    public required string PackagingProducer { get; init; }

    public bool IsExport { get; init; }

    public required Status Status { get; init; }

    public required Legacy Legacy { get; init; }

    public DateTime CreatedAt { get; init; }

    public required User CreatedBy { get; init; }

    public DateTime UpdatedAt { get; init; }

    public required User UpdatedBy { get; init; }
}
