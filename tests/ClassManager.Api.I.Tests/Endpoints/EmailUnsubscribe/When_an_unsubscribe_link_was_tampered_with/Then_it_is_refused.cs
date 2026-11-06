using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Api.I.Tests.Endpoints.EmailUnsubscribe.When_an_unsubscribe_link_was_tampered_with;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var token = EmailUnsubscribeRequests.UnsubscribeTokenOf(await fixture.SendOrderReadyEmailAsync(scenario));
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostUnsubscribeAsync(token[..^2] + "AA");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(NotificationErrorCodes.InvalidUnsubscribeLink);
    }
}
