using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UpdateMyNotificationPreference_turns_a_channel_off;

public sealed class Then_an_opt_out_is_stored
{
    [Fact]
    public async Task Then_an_opt_out_is_stored_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder();

        var response = await builder.BuildUpdate().ExecuteAsync(
            new UpdateMyNotificationPreferenceCommand(NotificationCategory.NewOrders, NotificationChannel.Push, IsEnabled: false), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        builder.OptOuts.OptOuts.Select(optOut => (optOut.AccountId, optOut.Category, optOut.Channel))
            .ShouldBe([(builder.AccountId, NotificationCategory.NewOrders, NotificationChannel.Push)]);
    }
}
