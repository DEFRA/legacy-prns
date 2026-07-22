using Microsoft.Extensions.Logging;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

internal sealed record LogEntry(
    LogLevel LogLevel,
    string Message,
    IReadOnlyList<KeyValuePair<string, object?>> State,
    Exception? Exception
);
