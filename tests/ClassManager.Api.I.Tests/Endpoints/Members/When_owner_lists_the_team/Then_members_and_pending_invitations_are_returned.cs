using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_lists_the_team;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_members_and_pending_invitations_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_members_and_pending_invitations_are_returned_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await scenario.Business.HttpClient.InviteAsync(email, BusinessRole.Viewer);

        var team = await scenario.Business.HttpClient.GetTeamAsync();

        team.Members.Select(member => member.Role).ShouldBe([BusinessRole.BranchOwner, BusinessRole.Instructor], ignoreOrder: true);
        team.Invitations.Select(invitation => invitation.Email).ShouldBe([email]);
    }
}
