using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Emails;

namespace ClassManager.Core.U.Tests.UseCases.Authentication.When_RequestSignUp_with_valid_data;

public sealed class Then_a_confirmation_link_is_emailed
{
    [Fact]
    public async Task Then_a_confirmation_link_is_emailed_Run()
    {
        var builder = new RequestSignUpUseCaseBuilder();

        await builder.Build().ExecuteAsync(RequestSignUpUseCaseBuilder.ValidCommand(), CancellationToken.None);

        builder.EmailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(message => message.To == TestData.OwnerEmail
                && message.Subject == EmailTexts.Spanish.SignUpConfirmation(TestData.BusinessName, RequestSignUpUseCaseBuilder.CompleteSignUpLink).Subject
                && message.TextBody.Contains(RequestSignUpUseCaseBuilder.CompleteSignUpLink)),
            It.IsAny<CancellationToken>()));
    }
}
