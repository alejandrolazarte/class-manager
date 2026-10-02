using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.I.Tests.Email.When_email_text_contains_html;

public sealed class Then_it_is_encoded
{
    [Fact]
    public void Then_it_is_encoded_Run()
    {
        var brand = new EmailBrand("<b>Club</b> {{Sections}}", null, null, null, IsBusiness: true);

        var html = BrandedEmailHtml.Render(new EmailContent("Eyebrow", "<script>alert(1)</script>", "Intro", "Footer"), brand);

        html.ShouldNotContain("<script>");
        html.ShouldContain("&lt;b&gt;Club&lt;/b&gt; {{Sections}}");
    }
}
