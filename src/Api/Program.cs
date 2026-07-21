using Defra.LegacyPrns.Api.Utils;
using Defra.LegacyPrns.Api.Utils.Health;
using Defra.LegacyPrns.Api.Utils.Logging;
using Elastic.CommonSchema.Serilog;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console(new EcsTextFormatter()).CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var integrationTest =
        args.Contains("--integrationTest=true")
        || builder.Configuration.GetValue("integrationTest", false)
        || builder.Environment.IsEnvironment("IntegrationTests");

    builder.Configuration.AddEnvironmentVariables();
    builder.Services.AddCustomTrustStore();
    builder.ConfigureLoggingAndTracing(integrationTest);
    builder.Services.AddProblemDetails();
    builder.Services.AddHealth();

    var app = builder.Build();

    if (!integrationTest)
        app.UseSerilogRequestLogging();

    app.UseHeaderPropagation();
    app.MapHealth();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    await Log.CloseAndFlushAsync();
}
