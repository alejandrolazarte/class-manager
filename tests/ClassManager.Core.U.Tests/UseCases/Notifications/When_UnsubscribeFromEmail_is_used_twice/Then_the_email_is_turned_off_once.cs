using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UnsubscribeFromEmail_is_used_twice;

public sealed class Then_the_email_is_turned_off_once
{
    [Fact]
    public async Task Then_the_email_is_turned_off_once_Run()
    {
        var builder = new EmailUnsubscribeUseCaseBuilder();
        var command = new UnsubscribeFromEmailCommand(EmailUnsubscribeUseCaseBuilder.ValidToken);

        await builder.BuildUnsubscribe().ExecuteAsync(command, CancellationToken.None);
        await builder.BuildUnsubscribe().ExecuteAsync(command, CancellationToken.None);

        builder.OptOuts.OptOuts.Count.ShouldBe(1);
    }
}
