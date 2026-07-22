using AwesomeAssertions;
using Defra.LegacyPrns.Api.Data;
using Defra.LegacyPrns.Api.Data.Entities;
using Defra.LegacyPrns.Api.Jobs;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
using Defra.LegacyPrns.Testing.Extensions.WireMock;
using Defra.LegacyPrns.Testing.Fixtures.PrnCommonBackend;
using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Defra.LegacyPrns.Api.IntegrationTests.Jobs;

public class MigrateLegacyPrnsTests : IntegrationTestBase
{
    [Fact]
    public async Task Run_WhenRawPrnsReturnedFromPrnCommonBackend_ShouldPersistLegacyPrns()
    {
        const int pageSize = 50;
        const string bearerValue = "wiremock-prn-common-backend";

        var prns = Enumerable.Range(1, 52).Select(PrnRawDataDtoFixture.Create).ToArray();
        await WireMockContext.WireMockAdminApi.StubTokenRequest(bearerValue, clientId: ClientIds.PrnCommonBackend);
        await WireMockContext.WireMockAdminApi.StubPrnCommonBackendRawDataRequest(
            1,
            pageSize,
            PaginatedPrnRawDataResponseFixture.Create(1, pageSize, prns.Take(pageSize).ToArray(), prns.Length),
            bearerValue
        );
        await WireMockContext.WireMockAdminApi.StubPrnCommonBackendRawDataRequest(
            2,
            pageSize,
            PaginatedPrnRawDataResponseFixture.Create(2, pageSize, prns.Skip(pageSize).ToArray(), prns.Length),
            bearerValue
        );

        await using var serviceProvider = CreateServiceProvider();
        serviceProvider.GetRequiredService<HeaderPropagationValues>().Headers = new Dictionary<string, StringValues>();
        var subject = serviceProvider.GetRequiredService<MigrateLegacyPrns>();

        await subject.Run(null, TestContext.Current.CancellationToken);

        var documents = await ReadLegacyPrns();

        documents.Should().HaveCount(prns.Length);
        documents.Select(x => x.Id).Should().OnlyContain(x => x != ObjectId.Empty);
        documents.Select(x => x.Id).Should().OnlyHaveUniqueItems();
        documents.Select(x => x.Legacy.ExternalId).Should().BeEquivalentTo(prns.Select(x => x.ExternalId));
        documents.Select(x => x.Legacy.PrnId).Should().Equal(prns.Select(x => x.Id));
        documents.Should().Contain(x => x.PrnNumber == prns[0].PrnNumber);

        var firstRunIds = documents.Select(x => x.Id).ToArray();

        await subject.Run(null, TestContext.Current.CancellationToken);

        var documentsAfterSecondRun = await ReadLegacyPrns();

        documentsAfterSecondRun.Should().HaveCount(prns.Length);
        documentsAfterSecondRun.Select(x => x.Id).Should().NotEqual(firstRunIds);
        documentsAfterSecondRun
            .Select(x => x.Legacy.ExternalId)
            .Should()
            .BeEquivalentTo(prns.Select(x => x.ExternalId));
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var config = new Dictionary<string, string?>
        {
            {
                $"{MongoDbOptions.SectionName}:DatabaseUri",
                "mongodb://127.0.0.1:27017/?replicaSet=rs0&directConnection=true&readPreference=secondaryPreferred"
            },
            { $"{MongoDbOptions.SectionName}:DatabaseName", "legacy-prns" },
            { $"{PrnCommonBackendOptions.SectionName}:BaseAddress", "http://localhost:9090/" },
            { $"{PrnCommonBackendOptions.SectionName}:TokenEndpoint", "http://localhost:9090/oauth2/v2.0/token" },
            { $"{PrnCommonBackendOptions.SectionName}:ClientId", ClientIds.PrnCommonBackend },
            { $"{PrnCommonBackendOptions.SectionName}:ClientSecret", "client_secret" },
            { $"{PrnCommonBackendOptions.SectionName}:Scope", "scope" },
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole());
        services.TryAddSingleton<HeaderPropagationValues>();
        services.AddMongo(configuration, validateConfigOnly: false);
        services.AddPrnCommonBackendService();
        services.AddJobs();

        return services.BuildServiceProvider();
    }

    private async Task<List<LegacyPrn>> ReadLegacyPrns() =>
        await LegacyPrns
            .Find(Builders<LegacyPrn>.Filter.Empty)
            .SortBy(x => x.Legacy.PrnId)
            .ToListAsync(TestContext.Current.CancellationToken);
}
