namespace Defra.LegacyPrns.Api.Data.Entities;

public record User
{
    public Guid? Id { get; init; }

    public string? Name { get; init; }
}
