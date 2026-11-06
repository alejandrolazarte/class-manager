using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Core.U.Tests.UseCases.Notifications.When_ListMyNotificationPreferences_in_a_student_session;

public sealed class Then_only_family_categories_are_listed
{
    [Fact]
    public async Task Then_only_family_categories_are_listed_Run()
    {
        var builder = new NotificationPreferencesUseCaseBuilder().AsTeamMember().WithFamilyAccess();

        var response = await builder.BuildList().ExecuteAsync(new ListMyNotificationPreferencesQuery(AccountKinds.Student), CancellationToken.None);

        response.Value!.Preferences.Select(preference => preference.Audience).Distinct().ShouldBe([NotificationAudience.Family]);
    }
}
