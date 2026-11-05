using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ListMyNotificationPreferences_has_an_opt_out;

public sealed class Then_only_that_channel_is_off
{
    [Fact]
    public async Task Then_only_that_channel_is_off_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder().WithFamilyAccess();
        builder.OptOuts.Add(NotificationOptOut.Create(builder.AccountId, NotificationCategory.Orders, NotificationChannel.Email, TestData.Now));

        var response = await builder.BuildList().ExecuteAsync(new ListMyNotificationPreferencesQuery(), CancellationToken.None);

        var turnedOff = response.Value!
            .SelectMany(preference => preference.Channels.Where(channel => !channel.IsEnabled).Select(channel => (preference.Category, channel.Channel)))
            .ToList();
        turnedOff.ShouldBe([(NotificationCategory.Orders, NotificationChannel.Email)]);
    }
}
