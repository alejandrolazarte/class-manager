namespace ClassManager.Notifications.U.Tests.Email.When_business_has_no_logo;

public sealed class Then_the_email_shows_its_initials
{
    [Fact]
    public void Then_the_email_shows_its_initials_Run()
    {
        var brand = new EmailBrand("Df Swimming Valencia", null, null, null);

        var html = BrandedEmailHtml.Render(new EmailContent("Eyebrow", "Title", "Intro", "Footer"), brand);

        html.ShouldContain(">DS</td>");
    }
}
