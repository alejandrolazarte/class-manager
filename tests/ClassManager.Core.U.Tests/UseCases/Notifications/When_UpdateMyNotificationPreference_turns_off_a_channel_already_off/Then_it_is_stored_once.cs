using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UpdateMyNotificationPreference_turns_off_a_channel_already_off;

public sealed class Then_it_is_stored_once
{
    [Fact]
    public async Task Then_it_is_stored_once_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder();
        var command = new UpdateMyNotificationPreferenceCommand(NotificationCategory.Orders, NotificationChannel.Email, IsEnabled: false);

        await builder.BuildUpdate().ExecuteAsync(command, CancellationToken.None);
        await builder.BuildUpdate().ExecuteAsync(command, CancellationToken.None);

        builder.OptOuts.OptOuts.Count.ShouldBe(1);
    }
}
