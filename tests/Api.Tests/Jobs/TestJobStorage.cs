using Hangfire;
using Hangfire.Storage;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

internal sealed class TestJobStorage : JobStorage
{
    public override IStorageConnection GetConnection() => new TestStorageConnection();

    public override IMonitoringApi GetMonitoringApi() => throw new NotSupportedException();
}
