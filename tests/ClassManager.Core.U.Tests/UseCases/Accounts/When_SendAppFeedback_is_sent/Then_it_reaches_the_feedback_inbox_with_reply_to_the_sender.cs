using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_SendAppFeedback_is_sent;

public sealed class Then_it_reaches_the_feedback_inbox_with_reply_to_the_sender
{
    [Fact]
    public async Task Then_it_reaches_the_feedback_inbox_with_reply_to_the_sender_Run()
    {
        const string message = "The calendar takes long to open.";
        var builder = new SendAppFeedbackUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(new SendAppFeedbackCommand(message, "web", "1.0.0"), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.EmailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(email =>
                email.To == SendAppFeedbackUseCaseBuilder.AppFeedbackInboxAddress
                && email.ReplyTo == TestData.OwnerEmail
                && email.TextBody.Contains(message)
                && email.TextBody.Contains(builder.Business.Name)),
            It.IsAny<CancellationToken>()));
    }
}
