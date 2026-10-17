using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_an_instructor_on_the_team_is_separated;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var (instructorId, _) = await fixture.SeedInstructorWhoIsAStudentAsync(scenario);
        await owner.InviteAsync(scenario.Email, BusinessRole.Instructor, instructorId);
        using var anonymous = fixture.ApiFactory.CreateClient();
        using (var accepted = await anonymous.PostAcceptInvitationAsync(
            fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(scenario.Email), StudentAppRequests.StudentAppPassword))
        {
            accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        using var response = await owner.PostSeparationAsync($"{ApiRoutes.Instructors}/{instructorId}", "Martina Gómez");

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(InstructorErrorCodes.OnTheTeam);
    }
}
