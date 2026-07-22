using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Authentication.AWS;

namespace Defra.LegacyPrns.Api.Data;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    private static readonly System.Threading.Lock s_bootstrapLock = new();
    private static bool s_bootstrapped;

    public static IServiceCollection AddMongo(
        this IServiceCollection services,
        IConfiguration configuration,
        bool validateConfigOnly
    )
    {
        services
            .AddOptions<MongoDbOptions>()
            .Bind(configuration.GetSection(MongoDbOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        if (validateConfigOnly)
            return services;

        BootstrapMongo();

        services.AddScoped<IDbContext, MongoDbContext>();
        services.AddTransient<ILegacyPrnRepository, LegacyPrnRepository>();
        services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var settings = MongoClientSettings.FromConnectionString(options.DatabaseUri);

            return new MongoClient(settings);
        });
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();

            return client.GetDatabase(options.DatabaseName);
        });

        return services;
    }

    private static void BootstrapMongo()
    {
        lock (s_bootstrapLock)
        {
            if (s_bootstrapped)
                return;

            MongoClientSettings.Extensions.AddAWSAuthentication();

            ConventionPack conventionPack =
            [
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(BsonType.String),
            ];

            ConventionRegistry.Register("LegacyPrnsMongoConventions", conventionPack, _ => true);
            s_bootstrapped = true;
        }
    }
}
