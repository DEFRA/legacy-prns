using AwesomeAssertions;
using Defra.LegacyPrns.Api.Data;
using Defra.LegacyPrns.Api.Data.Entities;
using Defra.LegacyPrns.Api.Jobs;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using Defra.LegacyPrns.Testing.Fixtures.PrnCommonBackend;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

public class MigrateLegacyPrnsTests
{
    [Fact]
    public async Task Run_WhenRawPrnsAreAvailable_ShouldMigratePagesAndLogBatchAndTotalCounts()
    {
        const string jobId = "job-123";
        const int pageSize = 50;

        var page1Items = Enumerable.Range(1, pageSize).Select(PrnRawDataDtoFixture.Create).ToArray();
        var page2Items = Enumerable.Range(51, 2).Select(PrnRawDataDtoFixture.Create).ToArray();
        var service = new FakePrnCommonBackendService(
            new Queue<PaginatedResponse<PrnRawDataDto>>([
                PaginatedPrnRawDataResponseFixture.Create(1, pageSize, page1Items, 52),
                PaginatedPrnRawDataResponseFixture.Create(2, pageSize, page2Items, 52),
            ])
        );
        var repository = new RecordingLegacyPrnRepository();
        var logger = new TestLogger<MigrateLegacyPrns>();
        var job = new MigrateLegacyPrns(service, repository, logger);
        var context = CreatePerformContext(jobId);

        await job.Run(context, TestContext.Current.CancellationToken);

        repository.DeleteAllCalls.Should().Be(1);
        service.Requests.Should().HaveCount(2);
        service.Requests.Should().OnlyContain(x => x.PageSize == pageSize);
        service.Requests.Select(x => x.Page).Should().Equal(1, 2);
        repository.Batches.Select(x => x.Count).Should().Equal(pageSize, 2);
        repository
            .Batches.SelectMany(x => x)
            .Select(x => x.Legacy.ExternalId)
            .Should()
            .BeEquivalentTo(page1Items.Concat(page2Items).Select(x => x.ExternalId));
        repository.Batches.SelectMany(x => x).Should().OnlyContain(x => x.Id == ObjectId.Empty);

        var deleteEntry = logger.Entries.Should().ContainSingle(x => HasState(x, "DeleteElapsedMilliseconds")).Subject;
        deleteEntry
            .Message.Should()
            .StartWith($"{nameof(MigrateLegacyPrns)} deleted existing legacy PRNs for Hangfire job {jobId} in ");
        AssertElapsedState(deleteEntry, "DeleteElapsedMilliseconds");

        var firstBatchEntry = logger.Entries.Should().ContainSingle(x => HasState(x, "BatchNumber", 1)).Subject;
        firstBatchEntry
            .Message.Should()
            .StartWith($"{nameof(MigrateLegacyPrns)} migrated 50 PRNs in batch 1 for Hangfire job {jobId} in ");
        AssertElapsedState(firstBatchEntry, "BatchElapsedMilliseconds");

        var secondBatchEntry = logger.Entries.Should().ContainSingle(x => HasState(x, "BatchNumber", 2)).Subject;
        secondBatchEntry
            .Message.Should()
            .StartWith($"{nameof(MigrateLegacyPrns)} migrated 2 PRNs in batch 2 for Hangfire job {jobId} in ");
        AssertElapsedState(secondBatchEntry, "BatchElapsedMilliseconds");

        var completionEntry = logger
            .Entries.Should()
            .ContainSingle(x => HasState(x, "TotalElapsedMilliseconds"))
            .Subject;
        completionEntry
            .Message.Should()
            .StartWith(
                $"{nameof(MigrateLegacyPrns)} completed for Hangfire job {jobId}. Total PRNs migrated: 52. Import completed in "
            );
        AssertElapsedState(completionEntry, "TotalElapsedMilliseconds");
    }

    [Fact]
    public async Task Run_WhenNoRawPrnsAreAvailable_ShouldLogTotalCount()
    {
        var service = new FakePrnCommonBackendService(
            new Queue<PaginatedResponse<PrnRawDataDto>>([
                PaginatedPrnRawDataResponseFixture.Create(1, 50, Array.Empty<PrnRawDataDto>(), 0),
            ])
        );
        var repository = new RecordingLegacyPrnRepository();
        var logger = new TestLogger<MigrateLegacyPrns>();
        var job = new MigrateLegacyPrns(service, repository, logger);

        await job.Run(null, TestContext.Current.CancellationToken);

        repository.DeleteAllCalls.Should().Be(1);
        repository.Batches.Should().BeEmpty();
        var deleteEntry = logger.Entries.Should().ContainSingle(x => HasState(x, "DeleteElapsedMilliseconds")).Subject;
        AssertElapsedState(deleteEntry, "DeleteElapsedMilliseconds");

        var completionEntry = logger
            .Entries.Should()
            .ContainSingle(x => HasState(x, "TotalElapsedMilliseconds"))
            .Subject;
        completionEntry
            .Message.Should()
            .StartWith(
                $"{nameof(MigrateLegacyPrns)} completed for Hangfire job unknown. Total PRNs migrated: 0. Import completed in "
            );
        AssertElapsedState(completionEntry, "TotalElapsedMilliseconds");
    }

    [Fact]
    public async Task Run_WhenMigrationFails_ShouldLogErrorWithDetailsAndRethrow()
    {
        var exception = new InvalidOperationException("Raw-data failed.");
        var service = new ThrowingPrnCommonBackendService(exception);
        var repository = new RecordingLegacyPrnRepository();
        var logger = new TestLogger<MigrateLegacyPrns>();
        var job = new MigrateLegacyPrns(service, repository, logger);

        var act = () => job.Run(null, TestContext.Current.CancellationToken);

        var thrown = await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage($"{nameof(MigrateLegacyPrns)} failed during migration for Hangfire job unknown.");
        thrown.Which.InnerException.Should().BeSameAs(exception);
        var entry = logger.Entries.Should().ContainSingle(x => x.LogLevel == LogLevel.Error).Subject;
        entry.LogLevel.Should().Be(LogLevel.Error);
        entry.Exception.Should().BeSameAs(exception);
        entry
            .Message.Should()
            .StartWith(
                $"{nameof(MigrateLegacyPrns)} failed during migration for Hangfire job unknown after migrating 0 PRNs in "
            );
        AssertElapsedState(entry, "TotalElapsedMilliseconds");
        repository.DeleteAllCalls.Should().Be(1);
    }

    private static PerformContext CreatePerformContext(string jobId)
    {
        var job = Job.FromExpression<MigrateLegacyPrns>(x => x.Run(null, CancellationToken.None));
        var backgroundJob = new BackgroundJob(jobId, job, DateTime.UtcNow);

        return new PerformContext(
            new TestJobStorage(),
            new TestStorageConnection(),
            backgroundJob,
            new TestJobCancellationToken()
        );
    }

    private static bool HasState(LogEntry entry, string key) => entry.State.Any(x => x.Key == key);

    private static bool HasState(LogEntry entry, string key, object value) =>
        entry.State.Any(x => x.Key == key && Equals(x.Value, value));

    private static void AssertElapsedState(LogEntry entry, string key)
    {
        var value = entry.State.Should().Contain(x => x.Key == key).Subject.Value;

        value.Should().BeOfType<long>().Which.Should().BeGreaterThanOrEqualTo(0);
    }

    private sealed class FakePrnCommonBackendService(Queue<PaginatedResponse<PrnRawDataDto>> responses)
        : IPrnCommonBackendService
    {
        public List<PaginatedRequest> Requests { get; } = [];

        public Task<PaginatedResponse<PrnRawDataDto>> ReadRawPrns(
            PaginatedRequest request,
            CancellationToken cancellationToken
        )
        {
            Requests.Add(request);

            return Task.FromResult(responses.Dequeue());
        }
    }

    private sealed class ThrowingPrnCommonBackendService(Exception exception) : IPrnCommonBackendService
    {
        public Task<PaginatedResponse<PrnRawDataDto>> ReadRawPrns(
            PaginatedRequest request,
            CancellationToken cancellationToken
        ) => throw exception;
    }

    private sealed class RecordingLegacyPrnRepository : ILegacyPrnRepository
    {
        public List<IReadOnlyCollection<LegacyPrn>> Batches { get; } = [];

        public int DeleteAllCalls { get; private set; }

        public Task DeleteAll(CancellationToken cancellationToken)
        {
            DeleteAllCalls++;

            return Task.CompletedTask;
        }

        public Task InsertMany(IReadOnlyCollection<LegacyPrn> legacyPrns, CancellationToken cancellationToken)
        {
            Batches.Add(legacyPrns);

            return Task.CompletedTask;
        }
    }
}
