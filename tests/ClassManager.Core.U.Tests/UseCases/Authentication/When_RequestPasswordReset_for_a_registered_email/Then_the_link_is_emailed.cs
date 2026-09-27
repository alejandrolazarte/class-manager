using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestPasswordReset_for_a_registered_email;

public sealed class Then_the_link_is_emailed
{
    [Fact]
    public async Task Then_the_link_is_emailed_Run()
    {
        const string token = "reset-token";
        const string link = "https://app.example.com/reset-password?token=reset-token";
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.CreatePasswordResetTokenAsync(TestData.OwnerEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        var webAppLinks = new Mock<IWebAppLinks>();
        webAppLinks.Setup(links => links.ResetPassword(token)).Returns(link);
        var emailSender = new Mock<IEmailSender>();
        var useCase = new RequestPasswordResetUseCase(identity.Object, emailSender.Object, webAppLinks.Object);

        await useCase.ExecuteAsync(new RequestPasswordResetCommand($" {TestData.OwnerEmail} "), CancellationToken.None);

        emailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(message => message.To == TestData.OwnerEmail && message.TextBody.Contains(link)),
            It.IsAny<CancellationToken>()));
    }
}
