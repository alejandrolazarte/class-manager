using ClassManager.Api.ErrorReporting;
using Microsoft.Data.SqlClient;
using Sentry.Protocol;

namespace ClassManager.Api.I.Tests.ErrorReporting.When_an_error_report_comes_from_a_sql_server_error;

public sealed class Then_its_message_is_removed
{
    [Fact]
    public void Then_its_message_is_removed_Run()
    {
        var sqlServerError = new SentryException
        {
            Type = typeof(SqlException).FullName,
            Value = "Cannot insert duplicate key row. The duplicate key value is (ana@example.com).",
        };
        var errorReport = new SentryEvent { SentryExceptions = [sqlServerError] };

        var scrubbedReport = ErrorReportPrivacy.RemovePersonalData(errorReport);

        scrubbedReport.SentryExceptions!.Single().Value.ShouldBe(ErrorReportPrivacy.RemovedDatabaseErrorMessage);
    }
}
