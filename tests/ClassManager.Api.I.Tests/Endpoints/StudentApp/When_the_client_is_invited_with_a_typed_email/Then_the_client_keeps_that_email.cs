using ClassManager.Core.UseCases.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_client_is_invited_with_a_typed_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_client_keeps_that_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_client_keeps_that_email_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.InstructorStudentFullName)).ClientId;
        var email = StudentAppRequests.UniqueStudentEmail();
        using var invitation = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email);

        var client = await instructors.Business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{clientId}", UriKind.Relative), ApiRequests.JsonOptions);

        client!.Email.ShouldBe(email);
    }
}
