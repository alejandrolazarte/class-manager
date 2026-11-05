using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.NotificationPreferences.When_a_student_reads_the_preferences;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_family_categories_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_family_categories_are_listed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var preferences = await scenario.Student.GetMyNotificationPreferencesAsync();

        preferences!.Preferences.Select(preference => preference.Audience).Distinct().ShouldBe([NotificationAudience.Family]);
    }
}
