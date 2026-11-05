using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UpdateMyNotificationPreference_turns_a_channel_back_on;

public sealed class Then_the_opt_out_is_removed
{
    [Fact]
    public async Task Then_the_opt_out_is_removed_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder();
        builder.OptOuts.Add(NotificationOptOut.Create(builder.AccountId, NotificationCategory.NewOrders, NotificationChannel.Push, TestData.Now));

        var response = await builder.BuildUpdate().ExecuteAsync(
            new UpdateMyNotificationPreferenceCommand(NotificationCategory.NewOrders, NotificationChannel.Push, IsEnabled: true), CancellationToken.None);

        response.Value!.Channels.ShouldAllBe(channel => channel.IsEnabled);
        builder.OptOuts.OptOuts.ShouldBeEmpty();
    }
}
