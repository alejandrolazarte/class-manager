namespace ClassManager.Notifications.U.Tests.Email.When_email_is_rendered;

public sealed class Then_outlook_for_windows_uses_arial
{
    [Fact]
    public void Then_outlook_for_windows_uses_arial_Run()
    {
        var html = BrandedEmailHtml.Render(
            new EmailContent("Eyebrow", "Title", "Intro", "Footer"), new EmailBrand("Club", null, null, null), new Uri("https://app.example.com/fonts/"));

        html.ShouldMatch(@"<!--\[if mso\]><style>[^<]*font-family:Arial,Helvetica,sans-serif !important[^<]*</style><!\[endif\]-->");
    }
}
