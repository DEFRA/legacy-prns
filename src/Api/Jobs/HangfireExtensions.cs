using System.Diagnostics.CodeAnalysis;
using Defra.LegacyPrns.Api.Data;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Options;

namespace Defra.LegacyPrns.Api.Jobs;

[ExcludeFromCodeCoverage]
public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireJobs(
        this IServiceCollection services,
        IConfiguration configuration,
        bool validateConfigOnly
    )
    {
        if (validateConfigOnly)
            return services;

        services
            .AddOptions<HangfireOptions>()
            .Bind(configuration.GetSection("Hangfire"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(sp => new BasicAuthenticationDashboardAuthorizationFilter(
            sp.GetRequiredService<IOptions<HangfireOptions>>().Value.Dashboard
        ));

        services
            .AddHangfire(
                (sp, hangfireConfiguration) =>
                {
                    var mongoOptions = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;

                    hangfireConfiguration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseMongoStorage(
                            mongoOptions.DatabaseUri,
                            mongoOptions.DatabaseName,
                            new MongoStorageOptions
                            {
                                MigrationOptions = new MongoMigrationOptions
                                {
                                    MigrationStrategy = new MigrateMongoMigrationStrategy(),
                                    BackupStrategy = new CollectionMongoBackupStrategy(),
                                },
                                Prefix = "hangfire.mongo",
                                CheckConnection = true,
                                ConnectionCheckTimeout = TimeSpan.FromMinutes(1),
                                QueuePollInterval = TimeSpan.FromSeconds(30),
                            }
                        );
                }
            )
            .AddHangfireServer();

        return services;
    }

    public static WebApplication MapHangfireJobsDashboard(this WebApplication app)
    {
        var dashboardOptions = app.Services.GetRequiredService<IOptions<HangfireOptions>>().Value.Dashboard;

        app.MapHangfireDashboard(
            "/hangfire",
            new DashboardOptions
            {
                Authorization = [app.Services.GetRequiredService<BasicAuthenticationDashboardAuthorizationFilter>()],
                StatsPollingInterval = dashboardOptions.StatsPollingIntervalMilliseconds,
            }
        );

        return app;
    }

    public static WebApplication RegisterRecurringJobs(this WebApplication app)
    {
        // Hangfire requires a valid cron for dashboard-triggerable recurring jobs; 30 February never occurs.
        app.Services.GetRequiredService<IRecurringJobManager>()
            .AddOrUpdate<MigrateLegacyPrns>(
                nameof(MigrateLegacyPrns),
                x => x.Run(null, CancellationToken.None),
                "0 0 30 2 *",
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
            );

        return app;
    }
}
