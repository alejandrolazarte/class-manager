using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UnsubscribeFromEmail_has_a_valid_link;

public sealed class Then_that_email_is_turned_off_in_its_business
{
    [Fact]
    public async Task Then_that_email_is_turned_off_in_its_business_Run()
    {
        var builder = new EmailUnsubscribeUseCaseBuilder();

        var response = await builder.BuildUnsubscribe().ExecuteAsync(
            new UnsubscribeFromEmailCommand(EmailUnsubscribeUseCaseBuilder.ValidToken), CancellationToken.None);

        response.Value!.IsUnsubscribed.ShouldBeTrue();
        builder.OptOuts.OptOuts.Select(optOut => (optOut.AccountId, optOut.Category, optOut.Channel))
            .ShouldBe([(builder.Unsubscription.AccountId, NotificationCategory.Orders, NotificationChannel.Email)]);
        builder.TenantScope.Verify(scope => scope.Establish(builder.Unsubscription.BusinessId));
    }
}
