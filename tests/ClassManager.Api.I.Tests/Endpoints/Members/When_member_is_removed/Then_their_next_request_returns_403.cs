using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_member_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_next_request_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_next_request_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var team = await scenario.Business.HttpClient.GetTeamAsync();
        var instructorMember = team.Members.Single(member => member.Role == BusinessRole.Instructor);
        using (var removeResponse = await scenario.Business.HttpClient.DeleteMemberAsync(instructorMember.Id))
        {
            removeResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        using var response = await scenario.Instructor.GetAsync(new Uri(ApiRoutes.Sessions, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
