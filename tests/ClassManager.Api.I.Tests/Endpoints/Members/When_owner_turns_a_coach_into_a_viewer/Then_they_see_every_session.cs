using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_turns_a_coach_into_a_viewer;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_see_every_session(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_see_every_session_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        var team = await scenario.Business.HttpClient.GetTeamAsync();
        var coachMember = team.Members.Single(member => member.Role == BusinessRole.Coach);
        using (var changeResponse = await scenario.Business.HttpClient.PutMemberAsync(coachMember.Id, BusinessRole.Viewer))
        {
            changeResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        var sessions = await scenario.Coach.GetDayAsync(CoachScenario.ClassDate);

        sessions!.Count.ShouldBe(2);
    }
}
