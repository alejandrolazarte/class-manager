using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ListMyNotificationPreferences_has_no_opt_outs;

public sealed class Then_every_channel_is_on
{
    [Fact]
    public async Task Then_every_channel_is_on_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder().AsTeamMember();

        var response = await builder.BuildList().ExecuteAsync(new ListMyNotificationPreferencesQuery(AccountKinds.Team), CancellationToken.None);

        response.Value!.Preferences.SelectMany(preference => preference.Channels).ShouldAllBe(channel => channel.IsEnabled);
    }
}
