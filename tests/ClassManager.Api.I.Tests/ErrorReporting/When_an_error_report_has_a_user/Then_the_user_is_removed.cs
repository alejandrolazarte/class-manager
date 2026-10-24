using ClassManager.Api.ErrorReporting;

namespace ClassManager.Api.I.Tests.ErrorReporting.When_an_error_report_has_a_user;

public sealed class Then_the_user_is_removed
{
    [Fact]
    public void Then_the_user_is_removed_Run()
    {
        var errorReport = new SentryEvent { User = new SentryUser { Email = "ana@example.com", IpAddress = "203.0.113.7" } };

        var scrubbedReport = ErrorReportPrivacy.RemovePersonalData(errorReport);

        scrubbedReport.User.Email.ShouldBeNull();
        scrubbedReport.User.IpAddress.ShouldBeNull();
    }
}
