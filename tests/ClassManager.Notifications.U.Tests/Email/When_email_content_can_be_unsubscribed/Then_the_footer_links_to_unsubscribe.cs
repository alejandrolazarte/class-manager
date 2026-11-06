namespace ClassManager.Notifications.U.Tests.Email.When_email_content_can_be_unsubscribed;

public sealed class Then_the_footer_links_to_unsubscribe
{
    [Fact]
    public void Then_the_footer_links_to_unsubscribe_Run()
    {
        const string link = "https://app.example.com/unsubscribe?token=abc&x=1";
        var content = new EmailContent("Pedido n.º 1", "Tu pedido está listo", "Ya está listo.", "Recibís este mail porque hiciste un pedido.")
        {
            Unsubscribe = new EmailAction("Dejar de recibir estos emails", link),
        };

        var html = BrandedEmailHtml.Render(content, NotificationsTestData.Brand, NotificationsTestData.WebFontsUrl);

        html.ShouldContain("<a href=\"https://app.example.com/unsubscribe?token=abc&amp;x=1\"");
        html.ShouldContain(">Dejar de recibir estos emails</a>");
    }
}
