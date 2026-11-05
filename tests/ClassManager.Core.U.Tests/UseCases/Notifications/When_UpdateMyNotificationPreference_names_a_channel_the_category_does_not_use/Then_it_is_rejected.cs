using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_UpdateMyNotificationPreference_names_a_channel_the_category_does_not_use;

public sealed class Then_it_is_rejected
{
    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder();

        var response = await builder.BuildUpdate().ExecuteAsync(
            new UpdateMyNotificationPreferenceCommand(NotificationCategory.News, NotificationChannel.Email, IsEnabled: false), CancellationToken.None);

        response.Error!.Code.ShouldBe(NotificationErrorCodes.ChannelNotInCategory);
        builder.OptOuts.OptOuts.ShouldBeEmpty();
    }
}
