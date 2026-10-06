using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_RequestEmailChange_is_valid;

public sealed class Then_the_link_goes_to_the_new_email
{
    private const string NewEmail = "laura.nueva@example.com";
    private const string Password = "a long passphrase";
    private const string Token = "change-token";
    private const string Link = "https://app.example.com/confirm-email-change?token=change-token";

    [Fact]
    public async Task Then_the_link_goes_to_the_new_email_Run()
    {
        var userId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.UserId).Returns(userId);
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.RequestEmailChangeAsync(userId, Password, NewEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EmailChangeRequested(Token, TestData.OwnerEmail));
        var webAppLinks = new Mock<IWebAppLinks>();
        webAppLinks.Setup(links => links.ConfirmEmailChange(Token)).Returns(Link);
        var emailSender = new Mock<IEmailSender>();
        var useCase = new RequestEmailChangeUseCase(currentUser.Object, identity.Object, emailSender.Object, webAppLinks.Object);

        await useCase.ExecuteAsync(new RequestEmailChangeCommand($" {NewEmail} ", Password), CancellationToken.None);

        emailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(message => message.To == NewEmail && message.TextBody.Contains(Link)),
            It.IsAny<CancellationToken>()));
    }
}
