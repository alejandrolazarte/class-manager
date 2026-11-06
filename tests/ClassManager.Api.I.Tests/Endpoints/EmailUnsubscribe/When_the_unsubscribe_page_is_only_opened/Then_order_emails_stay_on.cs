using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.EmailUnsubscribe.When_the_unsubscribe_page_is_only_opened;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_order_emails_stay_on(ApiFixture fixture)
{
    [Fact]
    public async Task Then_order_emails_stay_on_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var token = EmailUnsubscribeRequests.UnsubscribeTokenOf(await fixture.SendOrderReadyEmailAsync(scenario));
        using var anonymous = fixture.ApiFactory.CreateClient();

        var unsubscription = await anonymous.GetUnsubscriptionAsync(token);

        unsubscription!.IsUnsubscribed.ShouldBeFalse();
        (await scenario.Student.GetMyNotificationPreferencesAsync())!.IsEnabled(NotificationCategory.Orders, NotificationChannel.Email).ShouldBeTrue();
    }
}
