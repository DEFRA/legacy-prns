using Hangfire;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

internal sealed class TestJobCancellationToken : IJobCancellationToken
{
    public CancellationToken ShutdownToken => CancellationToken.None;

    public void ThrowIfCancellationRequested() { }
}
