using Defra.LegacyPrns.Api.Data.Entities;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.IntegrationTests;

[Trait("Category", "IntegrationTests")]
[Collection("Integration Tests")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private static readonly Uri s_baseAddress = new("http://localhost:8085");

    public required WireMockContext WireMockContext { get; set; }

    public required IMongoCollection<LegacyPrn> LegacyPrns { get; set; }

    public async ValueTask InitializeAsync()
    {
        WireMockContext = new WireMockContext();

        await WireMockContext.InitializeAsync();

        LegacyPrns = GetMongoCollection<LegacyPrn>();
        await DeleteMany(LegacyPrns);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }

    protected static HttpClient CreateClient(Uri? baseAddress = null) =>
        new() { BaseAddress = baseAddress ?? s_baseAddress };

    protected static IMongoDatabase GetMongoDatabase()
    {
        var settings = MongoClientSettings.FromConnectionString(
            "mongodb://127.0.0.1:27017/?replicaSet=rs0&directConnection=true&readPreference=secondaryPreferred"
        );
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        settings.ConnectTimeout = TimeSpan.FromSeconds(5);
        settings.SocketTimeout = TimeSpan.FromSeconds(5);

        return new MongoClient(settings).GetDatabase("legacy-prns");
    }

    private static IMongoCollection<T> GetMongoCollection<T>() => GetMongoDatabase().GetCollection<T>(typeof(T).Name);

    private static async Task DeleteMany<T>(IMongoCollection<T> collection) =>
        await collection.DeleteManyAsync(FilterDefinition<T>.Empty, TestContext.Current.CancellationToken);
}
