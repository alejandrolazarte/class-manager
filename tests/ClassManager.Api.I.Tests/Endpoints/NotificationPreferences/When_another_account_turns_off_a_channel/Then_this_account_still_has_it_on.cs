using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.NotificationPreferences.When_another_account_turns_off_a_channel;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_this_account_still_has_it_on(ApiFixture fixture)
{
    [Fact]
    public async Task Then_this_account_still_has_it_on_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Instructors.Instructor.TurnOffAsync(NotificationCategory.Attendance, NotificationChannel.Push);

        var preferences = await scenario.Instructors.Business.HttpClient.GetMyNotificationPreferencesAsync();

        preferences!.IsEnabled(NotificationCategory.Attendance, NotificationChannel.Push).ShouldBeTrue();
    }
}
