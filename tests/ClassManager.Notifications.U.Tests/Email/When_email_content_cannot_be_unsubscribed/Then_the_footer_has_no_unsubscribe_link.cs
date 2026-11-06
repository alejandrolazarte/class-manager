namespace ClassManager.Notifications.U.Tests.Email.When_email_content_cannot_be_unsubscribed;

public sealed class Then_the_footer_has_no_unsubscribe_link
{
    [Fact]
    public void Then_the_footer_has_no_unsubscribe_link_Run()
    {
        var content = new EmailContent("Cuenta", "Cambiá tu contraseña", "Tocá el botón.", "Footer");

        var html = BrandedEmailHtml.Render(content, NotificationsTestData.Brand, NotificationsTestData.WebFontsUrl);

        html.ShouldNotContain("unsubscribe");
    }
}
