using Hangfire;
using Hangfire.Server;

namespace Defra.LegacyPrns.Api.Jobs;

public class MigrateLegacyPrns(ILogger<MigrateLegacyPrns> logger)
{
    [JobDisplayName(nameof(MigrateLegacyPrns))]
    public async Task Run(PerformContext? context, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        logger.LogInformation(
            "{JobName} completed for Hangfire job {HangfireJobId}.",
            nameof(MigrateLegacyPrns),
            context?.BackgroundJob.Id ?? "unknown"
        );
    }
}
