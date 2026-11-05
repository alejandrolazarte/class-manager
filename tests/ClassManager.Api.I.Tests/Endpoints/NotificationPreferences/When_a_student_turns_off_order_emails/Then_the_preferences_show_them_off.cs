using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.NotificationPreferences.When_a_student_turns_off_order_emails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_preferences_show_them_off(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_preferences_show_them_off_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.PutMyNotificationPreferenceAsync(NotificationCategory.Orders, NotificationChannel.Email, isEnabled: false);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var preferences = await scenario.Student.GetMyNotificationPreferencesAsync();
        preferences!.IsEnabled(NotificationCategory.Orders, NotificationChannel.Email).ShouldBeFalse();
        preferences!.IsEnabled(NotificationCategory.Orders, NotificationChannel.Push).ShouldBeTrue();
    }
}
