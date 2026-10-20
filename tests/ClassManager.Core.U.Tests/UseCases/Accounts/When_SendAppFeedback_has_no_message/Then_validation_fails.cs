using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_SendAppFeedback_has_no_message;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var builder = new SendAppFeedbackUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(new SendAppFeedbackCommand("   ", "web", "1.0.0"), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(SendAppFeedbackCommand.Message));
        builder.EmailSender.Verify(sender => sender.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
