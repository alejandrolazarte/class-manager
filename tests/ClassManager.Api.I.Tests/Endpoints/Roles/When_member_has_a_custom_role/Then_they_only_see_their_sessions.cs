using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_member_has_a_custom_role;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_only_see_their_sessions(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_only_see_their_sessions_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        var role = await fixture.SeedCustomRoleAsync(
            scenario.Business.Business.Id, [Permissions.Business.View, Permissions.Sessions.ViewOwn, Permissions.Payments.Record]);
        var member = await fixture.SeedMemberAsync(scenario.Business.Business.Id, MemberRole.Custom(role), scenario.OtherInstructorId);

        var sessions = await member.GetDayAsync(CoachScenario.ClassDate);

        sessions!.Select(session => session.ClassGroupName).ShouldBe([CoachScenario.OtherClassGroupName]);
    }
}
