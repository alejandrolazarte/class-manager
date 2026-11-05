using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.NotificationPreferences.When_an_owner_who_also_studies_reads_the_preferences;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_team_and_family_categories_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_team_and_family_categories_are_listed_Run()
    {
        var ownerAndStudent = await fixture.SeedOwnerWhoIsAlsoStudentAsync();

        var preferences = await ownerAndStudent.Student.Student.GetMyNotificationPreferencesAsync();

        preferences!.Select(preference => preference.Audience).Distinct().ShouldBe([NotificationAudience.Team, NotificationAudience.Family], ignoreOrder: true);
    }
}
