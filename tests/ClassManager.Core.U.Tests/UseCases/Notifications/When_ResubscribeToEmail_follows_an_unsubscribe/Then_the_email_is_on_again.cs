using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ResubscribeToEmail_follows_an_unsubscribe;

public sealed class Then_the_email_is_on_again
{
    [Fact]
    public async Task Then_the_email_is_on_again_Run()
    {
        var builder = new EmailUnsubscribeUseCaseBuilder();
        await builder.BuildUnsubscribe().ExecuteAsync(new UnsubscribeFromEmailCommand(EmailUnsubscribeUseCaseBuilder.ValidToken), CancellationToken.None);

        var response = await builder.BuildResubscribe().ExecuteAsync(
            new ResubscribeToEmailCommand(EmailUnsubscribeUseCaseBuilder.ValidToken), CancellationToken.None);

        response.Value!.IsUnsubscribed.ShouldBeFalse();
        builder.OptOuts.OptOuts.ShouldBeEmpty();
    }
}
