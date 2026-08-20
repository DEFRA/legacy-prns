using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

internal sealed class TestStorageConnection : IStorageConnection
{
    public void Dispose() { }

    public IWriteOnlyTransaction CreateWriteTransaction() => throw new NotSupportedException();

    public IDisposable AcquireDistributedLock(string resource, TimeSpan timeout) => throw new NotSupportedException();

    public string CreateExpiredJob(
        Job job,
        IDictionary<string, string> parameters,
        DateTime createdAt,
        TimeSpan expireIn
    ) => throw new NotSupportedException();

    public IFetchedJob FetchNextJob(string[] queues, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public void SetJobParameter(string id, string name, string value) => throw new NotSupportedException();

    public string GetJobParameter(string id, string name) => throw new NotSupportedException();

    public JobData GetJobData(string jobId) => throw new NotSupportedException();

    public StateData GetStateData(string jobId) => throw new NotSupportedException();

    public void AnnounceServer(string serverId, ServerContext context) => throw new NotSupportedException();

    public void RemoveServer(string serverId) => throw new NotSupportedException();

    public void Heartbeat(string serverId) => throw new NotSupportedException();

    public int RemoveTimedOutServers(TimeSpan timeOut) => throw new NotSupportedException();

    public HashSet<string> GetAllItemsFromSet(string key) => throw new NotSupportedException();

    public string GetFirstByLowestScoreFromSet(string key, double fromScore, double toScore) =>
        throw new NotSupportedException();

    public void SetRangeInHash(string key, IEnumerable<KeyValuePair<string, string>> keyValuePairs) =>
        throw new NotSupportedException();

    public Dictionary<string, string> GetAllEntriesFromHash(string key) => throw new NotSupportedException();
}
