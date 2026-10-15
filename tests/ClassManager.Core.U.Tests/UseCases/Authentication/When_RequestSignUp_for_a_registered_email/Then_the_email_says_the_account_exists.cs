using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Emails;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_for_a_registered_email;

public sealed class Then_the_email_says_the_account_exists
{
    [Fact]
    public async Task Then_the_email_says_the_account_exists_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();
        builder.Identity
            .Setup(service => service.IsEmailRegisteredAsync(TestData.OwnerEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await builder.Build().ExecuteAsync(RequestSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.EmailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(message => message.Subject == EmailTexts.Spanish.SignUpAccountExists(RequestSignUpUseCaseBuilder.SignInLink).Subject
                && message.TextBody.Contains(RequestSignUpUseCaseBuilder.SignInLink)),
            It.IsAny<CancellationToken>()));
        builder.AddedPendingSignUp.ShouldBeNull();
    }
}
