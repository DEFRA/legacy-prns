using System.Diagnostics;
using Defra.LegacyPrns.Api.Data;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using Hangfire;
using Hangfire.Server;

namespace Defra.LegacyPrns.Api.Jobs;

public class MigrateLegacyPrns(
    IPrnCommonBackendService prnCommonBackendService,
    ILegacyPrnRepository legacyPrnRepository,
    ILogger<MigrateLegacyPrns> logger
)
{
    private const int PageSize = 50;

    [JobDisplayName(nameof(MigrateLegacyPrns))]
    public async Task Run(PerformContext? context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string unknownHangfireJobId = "unknown";
        var hangfireJobId = context?.BackgroundJob.Id ?? unknownHangfireJobId;
        var page = 1;
        var totalMigratedPrns = 0;
        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            var deleteStopwatch = Stopwatch.StartNew();
            await legacyPrnRepository.DeleteAll(cancellationToken);
            deleteStopwatch.Stop();
            logger.LogInformation(
                "{JobName} deleted existing legacy PRNs for Hangfire job {HangfireJobId} in {DeleteElapsedMilliseconds} ms.",
                nameof(MigrateLegacyPrns),
                hangfireJobId,
                deleteStopwatch.ElapsedMilliseconds
            );

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batchStopwatch = Stopwatch.StartNew();
                var response = await prnCommonBackendService.ReadRawPrns(
                    new PaginatedRequest { Page = page, PageSize = PageSize },
                    cancellationToken
                );
                var prns = response.Items ?? [];

                if (prns.Length == 0)
                    break;

                var legacyPrns = prns.Select(prn => prn.ToLegacyPrn()).ToArray();
                await legacyPrnRepository.InsertMany(legacyPrns, cancellationToken);
                batchStopwatch.Stop();

                totalMigratedPrns += legacyPrns.Length;
                logger.LogInformation(
                    "{JobName} migrated {MigratedPrnCount} PRNs in batch {BatchNumber} for Hangfire job {HangfireJobId} in {BatchElapsedMilliseconds} ms.",
                    nameof(MigrateLegacyPrns),
                    legacyPrns.Length,
                    page,
                    hangfireJobId,
                    batchStopwatch.ElapsedMilliseconds
                );

                if (page >= response.PageCount)
                    break;

                page++;
            }

            totalStopwatch.Stop();
            logger.LogInformation(
                "{JobName} completed for Hangfire job {HangfireJobId}. Total PRNs migrated: {TotalMigratedPrnCount}. Import completed in {TotalElapsedMilliseconds} ms.",
                nameof(MigrateLegacyPrns),
                hangfireJobId,
                totalMigratedPrns,
                totalStopwatch.ElapsedMilliseconds
            );
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            totalStopwatch.Stop();
            logger.LogError(
                exception,
                "{JobName} failed during migration for Hangfire job {HangfireJobId} after migrating {TotalMigratedPrnCount} PRNs in {TotalElapsedMilliseconds} ms.",
                nameof(MigrateLegacyPrns),
                hangfireJobId,
                totalMigratedPrns,
                totalStopwatch.ElapsedMilliseconds
            );

            throw new InvalidOperationException(
                $"{nameof(MigrateLegacyPrns)} failed during migration for Hangfire job {hangfireJobId}.",
                exception
            );
        }
    }
}
