using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UnsubscribeFromEmail_has_a_link_that_does_not_verify;

public sealed class Then_it_is_refused
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var builder = new EmailUnsubscribeUseCaseBuilder();

        var response = await builder.BuildUnsubscribe().ExecuteAsync(new UnsubscribeFromEmailCommand("tampered"), CancellationToken.None);

        response.Error!.Code.ShouldBe(NotificationErrorCodes.InvalidUnsubscribeLink);
        builder.OptOuts.OptOuts.ShouldBeEmpty();
    }
}
