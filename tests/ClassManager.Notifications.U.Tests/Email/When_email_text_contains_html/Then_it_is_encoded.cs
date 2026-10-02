namespace ClassManager.Notifications.U.Tests.Email.When_email_text_contains_html;

public sealed class Then_it_is_encoded
{
    [Fact]
    public void Then_it_is_encoded_Run()
    {
        var brand = new EmailBrand("<b>Club</b> {{Sections}}", null, null, null);

        var html = BrandedEmailHtml.Render(new EmailContent("Eyebrow", "<script>alert(1)</script>", "Intro", "Footer"), brand);

        html.ShouldNotContain("<script>");
        html.ShouldContain("&lt;b&gt;Club&lt;/b&gt; {{Sections}}");
    }
}
