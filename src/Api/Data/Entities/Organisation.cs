namespace Defra.LegacyPrns.Api.Data.Entities;

public record Organisation
{
    public Guid Id { get; init; }

    public required string Name { get; init; }
}
