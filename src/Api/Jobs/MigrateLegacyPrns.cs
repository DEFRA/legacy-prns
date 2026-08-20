using Hangfire;
using Hangfire.Server;

namespace Defra.LegacyPrns.Api.Jobs;

public class MigrateLegacyPrns(ILogger<MigrateLegacyPrns> logger)
{
    [JobDisplayName(nameof(MigrateLegacyPrns))]
    public Task Run(PerformContext? context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation(
            "{JobName} completed for Hangfire job {HangfireJobId}.",
            nameof(MigrateLegacyPrns),
            context?.BackgroundJob.Id ?? "unknown"
        );

        return Task.CompletedTask;
    }
}
