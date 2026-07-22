namespace Defra.LegacyPrns.Api.Data.Entities;

public record StatusHistoryLegacy
{
    public Guid CreatedByOrganisationId { get; init; }

    public int PrnStatusIdFk { get; init; }
}
