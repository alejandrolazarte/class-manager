using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.EmailUnsubscribe.When_the_unsubscribe_link_of_an_order_email_is_used;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_order_emails_are_turned_off(ApiFixture fixture)
{
    [Fact]
    public async Task Then_order_emails_are_turned_off_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var token = EmailUnsubscribeRequests.UnsubscribeTokenOf(await fixture.SendOrderReadyEmailAsync(scenario));
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostUnsubscribeAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await scenario.Student.GetMyNotificationPreferencesAsync())!.IsEnabled(NotificationCategory.Orders, NotificationChannel.Email).ShouldBeFalse();
    }
}
