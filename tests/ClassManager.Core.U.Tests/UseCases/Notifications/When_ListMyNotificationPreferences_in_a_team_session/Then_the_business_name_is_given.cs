using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ListMyNotificationPreferences_in_a_team_session;

public sealed class Then_the_business_name_is_given
{
    [Fact]
    public async Task Then_the_business_name_is_given_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder().AsTeamMember();

        var response = await builder.BuildList().ExecuteAsync(new ListMyNotificationPreferencesQuery(AccountKinds.Team), CancellationToken.None);

        response.Value!.BusinessName.ShouldBe(TestData.BusinessName);
    }
}
