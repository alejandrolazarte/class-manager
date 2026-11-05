using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ListMyNotificationPreferences_for_a_family_account;

public sealed class Then_team_categories_are_not_listed
{
    [Fact]
    public async Task Then_team_categories_are_not_listed_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder().WithFamilyAccess();

        var response = await builder.BuildList().ExecuteAsync(new ListMyNotificationPreferencesQuery(), CancellationToken.None);

        response.Value!.ShouldNotContain(preference => preference.Audience == NotificationAudience.Team);
    }
}
