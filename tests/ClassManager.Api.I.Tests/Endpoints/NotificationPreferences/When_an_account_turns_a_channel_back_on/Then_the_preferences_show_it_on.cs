using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.NotificationPreferences.When_an_account_turns_a_channel_back_on;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_preferences_show_it_on(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_preferences_show_it_on_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Student.TurnOffAsync(NotificationCategory.ClassChanges, NotificationChannel.Push);

        (await scenario.Student.PutMyNotificationPreferenceAsync(NotificationCategory.ClassChanges, NotificationChannel.Push, isEnabled: true)).EnsureSuccessStatusCode();

        (await scenario.Student.GetMyNotificationPreferencesAsync())!.IsEnabled(NotificationCategory.ClassChanges, NotificationChannel.Push).ShouldBeTrue();
    }
}
