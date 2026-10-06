using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_client_is_invited_with_a_typed_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_keeps_that_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_keeps_that_email_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        var email = StudentAppRequests.UniqueStudentEmail();
        using var invitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email);

        var client = await coaches.Business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{clientId}", UriKind.Relative), ApiRequests.JsonOptions);

        client!.Email.ShouldBe(email);
    }
}
