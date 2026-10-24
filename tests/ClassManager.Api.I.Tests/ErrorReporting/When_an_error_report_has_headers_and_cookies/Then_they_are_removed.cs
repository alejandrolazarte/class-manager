using ClassManager.Api.ErrorReporting;

namespace ClassManager.Api.I.Tests.ErrorReporting.When_an_error_report_has_headers_and_cookies;

public sealed class Then_they_are_removed
{
    [Fact]
    public void Then_they_are_removed_Run()
    {
        var request = new SentryRequest { Cookies = "session=secret", Data = "{\"name\":\"Ana\"}" };
        request.Headers["Authorization"] = "Bearer secret";
        var errorReport = new SentryEvent { Request = request };

        var scrubbedReport = ErrorReportPrivacy.RemovePersonalData(errorReport);

        scrubbedReport.Request.Headers.ShouldBeEmpty();
        scrubbedReport.Request.Cookies.ShouldBeNull();
        scrubbedReport.Request.Data.ShouldBeNull();
    }
}
