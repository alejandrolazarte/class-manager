using Sentry.Extensions.Logging;

namespace ClassManager.Api.ErrorReporting;

internal static class ErrorReportingExtensions
{
    private const string EntityFrameworkLogCategory = "Microsoft.EntityFrameworkCore";

    public static WebApplicationBuilder AddErrorReporting(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseSentry(options =>
        {
            options.SendDefaultPii = false;
            options.MinimumBreadcrumbLevel = LogLevel.Warning;
            options.DisableDiagnosticSourceIntegration();
            options.SetBeforeSend(ErrorReportPrivacy.RemovePersonalData);
            options.AddLogEntryFilter((category, _, _, _) => category.StartsWith(EntityFrameworkLogCategory, StringComparison.Ordinal));
        });
        return builder;
    }
}
