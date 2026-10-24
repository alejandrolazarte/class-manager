using ClassManager.Api.ErrorReporting;

namespace ClassManager.Api.I.Tests.ErrorReporting.When_an_error_report_has_a_query_string;

public sealed class Then_the_query_string_is_removed
{
    private const string PagePath = "https://api.laztty.com/api/auth/password-reset";

    [Fact]
    public void Then_the_query_string_is_removed_Run()
    {
        var errorReport = new SentryEvent
        {
            Request = new SentryRequest { Url = $"{PagePath}?email=ana@example.com", QueryString = "?email=ana@example.com" },
        };

        var scrubbedReport = ErrorReportPrivacy.RemovePersonalData(errorReport);

        scrubbedReport.Request.Url.ShouldBe(PagePath);
        scrubbedReport.Request.QueryString.ShouldBeNull();
    }
}
