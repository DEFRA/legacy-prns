namespace Defra.LegacyPrns.Api.Data.Entities;

public record Status
{
    public string? CurrentStatus { get; init; }

    public DateTime? CurrentStatusAt { get; init; }

    public StatusHistory[] History { get; init; } = [];
}
