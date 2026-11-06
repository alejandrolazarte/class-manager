using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.EmailUnsubscribe.When_someone_resubscribes_from_the_unsubscribe_page;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_order_emails_are_on_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_order_emails_are_on_again_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var token = EmailUnsubscribeRequests.UnsubscribeTokenOf(await fixture.SendOrderReadyEmailAsync(scenario));
        using var anonymous = fixture.ApiFactory.CreateClient();
        (await anonymous.PostUnsubscribeAsync(token)).EnsureSuccessStatusCode();

        using var response = await anonymous.PostResubscribeAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await scenario.Student.GetMyNotificationPreferencesAsync())!.IsEnabled(NotificationCategory.Orders, NotificationChannel.Email).ShouldBeTrue();
    }
}
