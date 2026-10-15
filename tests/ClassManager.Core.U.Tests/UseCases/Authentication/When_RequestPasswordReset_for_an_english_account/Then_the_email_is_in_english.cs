using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Emails;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestPasswordReset_for_an_english_account;

public sealed class Then_the_email_is_in_english
{
    [Fact]
    public async Task Then_the_email_is_in_english_Run()
    {
        const string token = "reset-token";
        const string link = "https://app.example.com/reset-password?token=reset-token";
        var userId = Guid.CreateVersion7();
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.CreatePasswordResetTokenAsync(TestData.OwnerEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        identity
            .Setup(service => service.FindUserIdByEmailAsync(TestData.OwnerEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userId);
        identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(userId, TestData.OwnerEmail, TestData.OwnerFullName, TestData.OwnerBirthDate, AccountLanguage.English)]);
        var webAppLinks = new Mock<IWebAppLinks>();
        webAppLinks.Setup(links => links.ResetPassword(token)).Returns(link);
        var emailSender = new Mock<IEmailSender>();
        var useCase = new RequestPasswordResetUseCase(identity.Object, emailSender.Object, webAppLinks.Object);

        await useCase.ExecuteAsync(new RequestPasswordResetCommand(TestData.OwnerEmail), CancellationToken.None);

        emailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(message => message.Subject == EmailTexts.English.PasswordReset(link).Subject),
            It.IsAny<CancellationToken>()));
    }
}
