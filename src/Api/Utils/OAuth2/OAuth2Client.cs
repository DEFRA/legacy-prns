namespace Defra.LegacyPrns.Api.Utils.OAuth2;

public class OAuth2Client(IHttpClientFactory httpClientFactory)
{
    public async Task<TokenResponse> RequestToken(OAuth2Options options, CancellationToken cancellationToken)
    {
        var values = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = options.ClientId,
            ["client_secret"] = options.ClientSecret,
        };

        if (options.Scope is not null)
            values.Add("scope", options.Scope);

        using var body = new FormUrlEncodedContent(values);
        var httpClient = httpClientFactory.CreateClient(nameof(OAuth2Client));
        using var response = await httpClient.PostAsync(options.TokenEndpoint, body, cancellationToken);

        response.EnsureSuccessStatusCode();

        var tokenResponse =
            await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Empty token response.");

        return tokenResponse;
    }
}
