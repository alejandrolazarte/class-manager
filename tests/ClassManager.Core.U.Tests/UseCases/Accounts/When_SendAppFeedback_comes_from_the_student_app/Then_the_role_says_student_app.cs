using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_SendAppFeedback_comes_from_the_student_app;

public sealed class Then_the_role_says_student_app
{
    [Fact]
    public async Task Then_the_role_says_student_app_Run()
    {
        var builder = new SendAppFeedbackUseCaseBuilder();
        builder.CurrentMember.Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>())).ReturnsAsync((MemberAccess?)null);

        await builder.Build().ExecuteAsync(new SendAppFeedbackCommand("Hello", "android", "1.0.0"), CancellationToken.None);

        builder.EmailSender.Verify(sender => sender.SendAsync(
            It.Is<EmailMessage>(email => email.TextBody.Contains(SendAppFeedbackUseCase.StudentAppRole)),
            It.IsAny<CancellationToken>()));
    }
}
