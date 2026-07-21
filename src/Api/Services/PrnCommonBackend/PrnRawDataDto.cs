using System.Text.Json.Serialization;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public record PrnRawDataDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("externalId")]
    public Guid ExternalId { get; init; }

    [JsonPropertyName("prnNumber")]
    public required string PrnNumber { get; init; }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; init; }

    [JsonPropertyName("organisationName")]
    public required string OrganisationName { get; init; }

    [JsonPropertyName("producerAgency")]
    public required string ProducerAgency { get; init; }

    [JsonPropertyName("reprocessorExporterAgency")]
    public required string ReprocessorExporterAgency { get; init; }

    [JsonPropertyName("prnStatusId")]
    public int PrnStatusId { get; init; }

    [JsonPropertyName("tonnageValue")]
    public int TonnageValue { get; init; }

    [JsonPropertyName("materialName")]
    public required string MaterialName { get; init; }

    [JsonPropertyName("issuerNotes")]
    public string? IssuerNotes { get; init; }

    [JsonPropertyName("issuerReference")]
    public required string IssuerReference { get; init; }

    [JsonPropertyName("prnSignatory")]
    public string? PrnSignatory { get; init; }

    [JsonPropertyName("prnSignatoryPosition")]
    public string? PrnSignatoryPosition { get; init; }

    [JsonPropertyName("signature")]
    public string? Signature { get; init; }

    [JsonPropertyName("issueDate")]
    public DateTime IssueDate { get; init; }

    [JsonPropertyName("processToBeUsed")]
    public string? ProcessToBeUsed { get; init; }

    [JsonPropertyName("decemberWaste")]
    public bool DecemberWaste { get; init; }

    [JsonPropertyName("statusUpdatedOn")]
    public DateTime? StatusUpdatedOn { get; init; }

    [JsonPropertyName("issuedByOrg")]
    public required string IssuedByOrg { get; init; }

    [JsonPropertyName("accreditationNumber")]
    public required string AccreditationNumber { get; init; }

    [JsonPropertyName("reprocessingSite")]
    public string? ReprocessingSite { get; init; }

    [JsonPropertyName("accreditationYear")]
    public required string AccreditationYear { get; init; }

    [JsonPropertyName("obligationYear")]
    public required string ObligationYear { get; init; }

    [JsonPropertyName("packagingProducer")]
    public required string PackagingProducer { get; init; }

    [JsonPropertyName("createdBy")]
    public string? CreatedBy { get; init; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; init; }

    [JsonPropertyName("lastUpdatedBy")]
    public Guid LastUpdatedBy { get; init; }

    [JsonPropertyName("lastUpdatedDate")]
    public DateTime LastUpdatedDate { get; init; }

    [JsonPropertyName("isExport")]
    public bool IsExport { get; init; }

    [JsonPropertyName("sourceSystemId")]
    public string? SourceSystemId { get; init; }

    [JsonPropertyName("prnStatusHistories")]
    public PrnStatusHistoryRawDataDto[] PrnStatusHistories { get; init; } = [];
}
