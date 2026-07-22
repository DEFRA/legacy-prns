using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.LegacyPrns.Api.Jobs;

[ExcludeFromCodeCoverage]
public class HangfireDashboardOptions
{
    private const string NonWhitespacePattern = @".*\S.*";

    [Required]
    [RegularExpression(NonWhitespacePattern)]
    public string Username { get; set; } = null!;

    [Required]
    [RegularExpression(NonWhitespacePattern)]
    public string Password { get; set; } = null!;

    [Range(1000, int.MaxValue)]
    public int StatsPollingIntervalMilliseconds { get; set; } = 60000;
}
