using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.LegacyPrns.Api.Data;

[ExcludeFromCodeCoverage]
public class MongoDbOptions
{
    public const string SectionName = "Mongo";

    [Required]
    public string DatabaseUri { get; set; } = null!;

    [Required]
    public string DatabaseName { get; set; } = null!;
}
