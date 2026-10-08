using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_pending_student_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_the_invited_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_the_invited_email_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.InstructorStudentFullName)).ClientId;
        var email = StudentAppRequests.UniqueStudentEmail();
        using (var invitation = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostCheckStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email));

        var checkedInvitation = await response.Content.ReadFromJsonAsync<CheckStudentAppInvitationResponse>(ApiRequests.JsonOptions);
        checkedInvitation.ShouldBe(new CheckStudentAppInvitationResponse(email, checkedInvitation!.BusinessName, HasAccount: false, checkedInvitation.FullName));
        checkedInvitation.FullName.ShouldNotBeNullOrEmpty();
    }
}
