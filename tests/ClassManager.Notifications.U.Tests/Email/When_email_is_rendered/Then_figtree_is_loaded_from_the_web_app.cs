namespace ClassManager.Notifications.U.Tests.Email.When_email_is_rendered;

public sealed class Then_figtree_is_loaded_from_the_web_app
{
    [Fact]
    public void Then_figtree_is_loaded_from_the_web_app_Run()
    {
        var webFontsUrl = new Uri("https://app.example.com/fonts");

        var html = BrandedEmailHtml.Render(new EmailContent("Eyebrow", "Title", "Intro", "Footer"), new EmailBrand("Club", null, null, null), webFontsUrl);

        html.ShouldContain("url('https://app.example.com/fonts/" + BrandedEmailHtml.WebFontFileNames[0] + "')");
        html.ShouldNotContain("fonts.googleapis.com");
    }
}
