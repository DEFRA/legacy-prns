using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Hangfire.Dashboard;
using Microsoft.Net.Http.Headers;

namespace Defra.LegacyPrns.Api.Jobs;

[ExcludeFromCodeCoverage]
public class BasicAuthenticationDashboardAuthorizationFilter(HangfireDashboardOptions options)
    : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var authorization = httpContext.Request.Headers.Authorization.ToString();

        if (
            string.IsNullOrWhiteSpace(authorization)
            || !AuthenticationHeaderValue.TryParse(authorization, out var header)
            || !"Basic".Equals(header.Scheme, StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(header.Parameter)
        )
        {
            return Challenge(httpContext);
        }

        if (!TryDecodeCredentials(header.Parameter, out var username, out var password))
            return Challenge(httpContext);

        if (CredentialsMatch(username, options.Username) && CredentialsMatch(password, options.Password))
            return true;

        return Challenge(httpContext);
    }

    private static bool TryDecodeCredentials(string parameter, out string username, out string password)
    {
        username = string.Empty;
        password = string.Empty;

        string decoded;

        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(parameter));
        }
        catch (FormatException)
        {
            return false;
        }

        var separatorIndex = decoded.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0)
            return false;

        username = decoded[..separatorIndex];
        password = decoded[(separatorIndex + 1)..];

        return true;
    }

    private static bool CredentialsMatch(string supplied, string expected)
    {
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        return suppliedBytes.Length == expectedBytes.Length
            && CryptographicOperations.FixedTimeEquals(suppliedBytes, expectedBytes);
    }

    private static bool Challenge(HttpContext httpContext)
    {
        httpContext.Response.Headers[HeaderNames.WWWAuthenticate] = "Basic realm=\"Hangfire\"";

        return false;
    }
}
