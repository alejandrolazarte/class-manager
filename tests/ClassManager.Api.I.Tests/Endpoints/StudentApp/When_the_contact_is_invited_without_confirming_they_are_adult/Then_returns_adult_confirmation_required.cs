using ClassManager.Core.Domain.Clients;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_the_contact_is_invited_without_confirming_they_are_adult;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_adult_confirmation_required(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_adult_confirmation_required_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.InstructorStudentFullName)).ClientId;

        using var response = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(
            clientId, StudentAppRequests.UniqueStudentEmail(), confirmsContactIsAdult: false);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(StudentAppErrorCodes.ContactAdultConfirmationRequired);
    }
}
