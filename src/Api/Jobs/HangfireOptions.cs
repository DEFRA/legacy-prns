using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Defra.LegacyPrns.Api.Jobs;

[ExcludeFromCodeCoverage]
public class HangfireOptions
{
    [Required]
    [ValidateObjectMembers]
    public HangfireDashboardOptions Dashboard { get; set; } = null!;
}
