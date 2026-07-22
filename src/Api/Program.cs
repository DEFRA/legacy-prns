using Defra.LegacyPrns.Api.Data;
using Defra.LegacyPrns.Api.Jobs;
using Defra.LegacyPrns.Api.Services.PrnCommonBackend;
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
    builder.Services.AddMongo(builder.Configuration, integrationTest);
    builder.Services.AddHealth(!integrationTest);
    builder.Services.AddPrnCommonBackendService();
    builder.Services.AddJobs();
    builder.Services.AddHangfireJobs(builder.Configuration, integrationTest);

    var app = builder.Build();

    app.UseRequestLogging(integrationTest);
    app.UseHeaderPropagation();
    app.MapHealth();
    app.MapHangfireJobsDashboard(integrationTest);
    app.RegisterRecurringJobs(integrationTest);

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
