namespace ClassManager.Notifications.U.Tests.Email.When_email_layout_is_in_another_language;

public sealed class Then_the_link_fallback_uses_it
{
    [Fact]
    public void Then_the_link_fallback_uses_it_Run()
    {
        var layout = new EmailLayoutTexts("en", "Total", "If the button doesn't work, copy this link:");
        var content = new EmailContent("Eyebrow", "Title", "Intro", "Footer")
        {
            Layout = layout,
            Action = new EmailAction("Accept", "https://app.example.com/accept", ShowsLinkFallback: true),
        };

        var html = BrandedEmailHtml.Render(content, new EmailBrand("Club", null, null, null), new Uri("https://app.example.com/fonts/"));

        html.ShouldContain("If the button doesn&#39;t work, copy this link:");
        html.ShouldContain("<html lang=\"en\">");
    }
}
