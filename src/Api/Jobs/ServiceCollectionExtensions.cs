namespace Defra.LegacyPrns.Api.Jobs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services.AddTransient<MigrateLegacyPrns>();

        return services;
    }
}
