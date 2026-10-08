using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_turns_an_instructor_into_a_viewer;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_see_every_session(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_see_every_session_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var team = await scenario.Business.HttpClient.GetTeamAsync();
        var instructorMember = team.Members.Single(member => member.Role == BusinessRole.Instructor);
        using (var changeResponse = await scenario.Business.HttpClient.PutMemberAsync(instructorMember.Id, BusinessRole.Viewer))
        {
            changeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var sessions = await scenario.Instructor.GetDayAsync(InstructorScenario.ClassDate);

        sessions!.Count.ShouldBe(2);
    }
}
