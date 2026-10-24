using Microsoft.Data.SqlClient;

namespace ClassManager.Api.ErrorReporting;

internal static class ErrorReportPrivacy
{
    public const string RemovedDatabaseErrorMessage = "Removed: SQL Server messages can contain the values of a row.";
    private const char QueryStringSeparator = '?';
    private static readonly string? SqlExceptionTypeName = typeof(SqlException).FullName;

    public static SentryEvent RemovePersonalData(SentryEvent errorReport)
    {
        errorReport.User = new SentryUser();
        RemovePersonalData(errorReport.Request);
        foreach (var exception in errorReport.SentryExceptions ?? [])
        {
            if (exception.Type == SqlExceptionTypeName)
            {
                exception.Value = RemovedDatabaseErrorMessage;
            }
        }

        return errorReport;
    }

    private static void RemovePersonalData(SentryRequest request)
    {
        request.Url = WithoutQueryString(request.Url);
        request.QueryString = null;
        request.Cookies = null;
        request.Data = null;
        request.Headers.Clear();
        request.Env.Clear();
    }

    private static string? WithoutQueryString(string? url)
    {
        var separatorIndex = url?.IndexOf(QueryStringSeparator, StringComparison.Ordinal) ?? -1;
        return separatorIndex < 0 ? url : url![..separatorIndex];
    }
}
