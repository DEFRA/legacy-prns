using System.Text.Json.Serialization;

namespace Defra.LegacyPrns.Api.Services.PrnCommonBackend;

public record PrnStatusHistoryRawDataDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; init; }

    [JsonPropertyName("createdByUser")]
    public Guid CreatedByUser { get; init; }

    [JsonPropertyName("createdByOrganisationId")]
    public Guid CreatedByOrganisationId { get; init; }

    [JsonPropertyName("prnStatusIdFk")]
    public int PrnStatusIdFk { get; init; }

    [JsonPropertyName("prnIdFk")]
    public int PrnIdFk { get; init; }

    [JsonPropertyName("comment")]
    public string? Comment { get; init; }

    [JsonPropertyName("obligationYear")]
    public string? ObligationYear { get; init; }
}
