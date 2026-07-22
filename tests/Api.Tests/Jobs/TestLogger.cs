using Microsoft.Extensions.Logging;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

internal sealed class TestLogger<T> : ILogger<T>
{
    public List<LogEntry> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var logState = state as IReadOnlyList<KeyValuePair<string, object?>> ?? [];
        Entries.Add(new LogEntry(logLevel, formatter(state, exception), logState, exception));
    }
}
