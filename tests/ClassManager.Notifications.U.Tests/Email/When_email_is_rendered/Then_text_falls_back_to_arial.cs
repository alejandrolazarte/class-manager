namespace ClassManager.Notifications.U.Tests.Email.When_email_is_rendered;

public sealed class Then_text_falls_back_to_arial
{
    [Fact]
    public void Then_text_falls_back_to_arial_Run()
    {
        var html = BrandedEmailHtml.Render(
            new EmailContent("Eyebrow", "Title", "Intro", "Footer"), new EmailBrand("Club", null, null, null), new Uri("https://app.example.com/fonts/"));

        html.ShouldContain("font-family:" + BrandedEmailHtml.FontFamily);
        BrandedEmailHtml.FontFamily.ShouldStartWith("Figtree,Arial");
    }
}
