namespace Defra.LegacyPrns.Api.Data.Entities;

public record Legacy
{
    public int PrnId { get; init; }

    public Guid ExternalId { get; init; }

    public string? SourceSystemId { get; init; }

    public int PrnStatusId { get; init; }

    public required string IssuerReference { get; init; }

    public string? ProcessToBeUsed { get; init; }
}
