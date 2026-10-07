using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_a_child_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_birth_date_typed_by_the_team_comes_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_birth_date_typed_by_the_team_comes_back_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var family = await business.HttpClient.RegisterFamilyAsync(StudentAppRequests.UniqueStudentEmail());
        var childEmail = StudentAppRequests.UniqueStudentEmail();
        using (var invitation = await business.HttpClient.PostChildAppInvitationAsync(
            family.Id, family.Students.Single().Id, childEmail, StudentAppRequests.ChildBirthDate))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostCheckStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(childEmail));

        var checkedInvitation = await response.Content.ReadFromJsonAsync<CheckStudentAppInvitationResponse>(ApiRequests.JsonOptions);
        checkedInvitation!.BirthDate.ShouldBe(StudentAppRequests.ChildBirthDate);
    }
}
