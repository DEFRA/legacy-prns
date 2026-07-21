namespace Defra.LegacyPrns.Api.IntegrationTests;

[Trait("Category", "IntegrationTests")]
public abstract class IntegrationTestBase
{
    private static readonly Uri s_baseAddress = new("http://localhost:8085");

    protected static HttpClient CreateClient(Uri? baseAddress = null) =>
        new() { BaseAddress = baseAddress ?? s_baseAddress };
}
