using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.NotificationPreferences.When_a_channel_the_category_does_not_use_is_turned_off;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_rejected(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.PutMyNotificationPreferenceAsync(NotificationCategory.News, NotificationChannel.Email, isEnabled: false);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(NotificationErrorCodes.ChannelNotInCategory);
    }
}
