namespace ClassManager.Notifications.U.Tests.Email.When_email_content_can_be_unsubscribed;

public sealed class Then_the_plain_text_ends_with_the_unsubscribe_link
{
    [Fact]
    public void Then_the_plain_text_ends_with_the_unsubscribe_link_Run()
    {
        const string link = "https://app.example.com/unsubscribe?token=abc";
        var content = new EmailContent("Pedido n.º 1", "Tu pedido está listo", "Ya está listo.", "Footer")
        {
            Unsubscribe = new EmailAction("Dejar de recibir estos emails", link),
        };

        content.ToPlainText().ShouldEndWith("Dejar de recibir estos emails: " + link);
    }
}
