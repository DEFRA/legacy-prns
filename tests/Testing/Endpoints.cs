using System.Diagnostics.CodeAnalysis;

namespace Defra.LegacyPrns.Testing;

[SuppressMessage(
    "Critical Code Smell",
    "S3218:Inner class members should not shadow outer class \"static\" or type members"
)]
public static class Endpoints
{
    public static class Health
    {
        public const string Ready = "health";

        public static string All() => $"{Ready}/all";
    }

    public static class Hangfire
    {
        public const string Dashboard = "hangfire";

        public static string RecurringJobs() => $"{Dashboard}/recurring";
    }
}
