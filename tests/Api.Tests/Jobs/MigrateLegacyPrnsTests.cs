using AwesomeAssertions;
using Defra.LegacyPrns.Api.Jobs;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

public class MigrateLegacyPrnsTests
{
    [Fact]
    public async Task Run_WhenJobCompletes_ShouldLogHangfireJobId()
    {
        const string jobId = "job-123";
        var logger = new TestLogger<MigrateLegacyPrns>();
        var job = new MigrateLegacyPrns(logger);
        var context = CreatePerformContext(jobId);

        await job.Run(context, TestContext.Current.CancellationToken);

        var entry = logger.Entries.Should().ContainSingle().Subject;
        entry.LogLevel.Should().Be(LogLevel.Information);
        entry.Message.Should().Be($"{nameof(MigrateLegacyPrns)} completed for Hangfire job {jobId}.");
        entry
            .State.Should()
            .Contain(x => x.Key == "JobName" && x.Value != null && x.Value.ToString() == nameof(MigrateLegacyPrns));
        entry.State.Should().Contain(x => x.Key == "HangfireJobId" && x.Value != null && x.Value.ToString() == jobId);
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
}
