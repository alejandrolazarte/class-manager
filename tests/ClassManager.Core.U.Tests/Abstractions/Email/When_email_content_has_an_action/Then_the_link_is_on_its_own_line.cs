using ClassManager.Core.Abstractions.Email;

namespace ClassManager.Core.U.Tests.Abstractions.Email.When_email_content_has_an_action;

public sealed class Then_the_link_is_on_its_own_line
{
    [Fact]
    public void Then_the_link_is_on_its_own_line_Run()
    {
        const string link = "https://app.example.com/accept-invitation?token=abc";
        var content = new EmailContent("Invitación", "Te invitaron", "Tocá el botón.", "Footer")
        {
            Action = new EmailAction("Aceptar invitación", link, ShowsLinkFallback: true),
        };

        content.ToPlainText().Split('\n').ShouldContain(link);
    }
}
