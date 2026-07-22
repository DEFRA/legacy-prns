using Defra.LegacyPrns.Api.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.Utils.Health;

public class MongoHealthCheck(IMongoDatabase database) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1),
                cancellationToken: cancellationToken
            );

            return HealthCheckResult.Healthy();
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy($"{MongoDbOptions.SectionName} health check failed.", exception);
        }
    }
}
