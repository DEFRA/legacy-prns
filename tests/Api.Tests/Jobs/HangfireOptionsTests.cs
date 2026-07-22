using AwesomeAssertions;
using Defra.LegacyPrns.Api.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Defra.LegacyPrns.Api.Tests.Jobs;

public class HangfireOptionsTests
{
    [Fact]
    public void Value_WhenDashboardCredentialsConfigured_ShouldReturnOptions()
    {
        const string username = "developer";
        const string password = "password";

        using var provider = BuildProvider([
            new("Hangfire:Dashboard:Username", username),
            new("Hangfire:Dashboard:Password", password),
        ]);

        var options = provider.GetRequiredService<IOptions<HangfireOptions>>().Value;

        options.Dashboard.Username.Should().Be(username);
        options.Dashboard.Password.Should().Be(password);
        options.Dashboard.StatsPollingIntervalMilliseconds.Should().Be(60000);
    }

    [Theory]
    [InlineData(null, "password")]
    [InlineData("", "password")]
    [InlineData("   ", "password")]
    [InlineData("developer", null)]
    [InlineData("developer", "")]
    [InlineData("developer", "   ")]
    public void Value_WhenDashboardCredentialsInvalid_ShouldThrowOptionsValidationException(
        string? username,
        string? password
    )
    {
        List<KeyValuePair<string, string?>> configurationValues = [];

        if (username is not null)
            configurationValues.Add(new("Hangfire:Dashboard:Username", username));

        if (password is not null)
            configurationValues.Add(new("Hangfire:Dashboard:Password", password));

        using var provider = BuildProvider(configurationValues);

        var act = () => provider.GetRequiredService<IOptions<HangfireOptions>>().Value;

        act.Should().Throw<OptionsValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void Value_WhenStatsPollingIntervalInvalid_ShouldThrowOptionsValidationException(
        int statsPollingIntervalMilliseconds
    )
    {
        using var provider = BuildProvider([
            new("Hangfire:Dashboard:Username", "developer"),
            new("Hangfire:Dashboard:Password", "password"),
            new("Hangfire:Dashboard:StatsPollingIntervalMilliseconds", statsPollingIntervalMilliseconds.ToString()),
        ]);

        var act = () => provider.GetRequiredService<IOptions<HangfireOptions>>().Value;

        act.Should().Throw<OptionsValidationException>();
    }

    [Fact]
    public void Value_WhenDashboardSectionMissing_ShouldThrowOptionsValidationException()
    {
        using var provider = BuildProvider([]);

        var act = () => provider.GetRequiredService<IOptions<HangfireOptions>>().Value;

        act.Should().Throw<OptionsValidationException>();
    }

    private static ServiceProvider BuildProvider(IEnumerable<KeyValuePair<string, string?>> configurationValues)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValues).Build();
        var services = new ServiceCollection();
        services.AddOptions<HangfireOptions>().Bind(configuration.GetSection("Hangfire")).ValidateDataAnnotations();

        return services.BuildServiceProvider();
    }
}
