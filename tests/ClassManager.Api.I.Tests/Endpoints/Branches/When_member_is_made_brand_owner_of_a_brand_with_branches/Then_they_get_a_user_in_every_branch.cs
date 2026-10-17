using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_member_is_made_brand_owner_of_a_brand_with_branches;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_get_a_user_in_every_branch(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_get_a_user_in_every_branch_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var branch = await business.HttpClient.CreateBranchAsync("DF Valencia");
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymous = fixture.ApiFactory.CreateClient();
        var memberTokens = await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var member = (await business.HttpClient.GetTeamAsync()).Members.Single(teamMember => teamMember.Role == BusinessRole.Viewer);

        (await business.HttpClient.PutBrandOwnerAsync(member.Id)).EnsureSuccessStatusCode();

        var switchedTokens = await anonymous.SwitchBranchAsync(memberTokens.RefreshToken, branch.BusinessId);
        var branchUserId = await fixture.FindUserIdByAccountAsync(branch.BusinessId, UserRows.AccountIdOf(memberTokens.AccessToken));
        (branchUserId.HasValue, UserRows.UserIdOf(switchedTokens.AccessToken)).ShouldBe((true, branchUserId));
    }
}
