using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_a_branch_is_created_in_a_brand_with_other_owners;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_brand_owner_gets_a_user_in_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_brand_owner_gets_a_user_in_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymous = fixture.ApiFactory.CreateClient();
        var memberTokens = await anonymous.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));
        var member = (await business.HttpClient.GetTeamAsync()).Members.Single(teamMember => teamMember.Role == BusinessRole.Viewer);
        (await business.HttpClient.PutBrandOwnerAsync(member.Id)).EnsureSuccessStatusCode();

        var branch = await business.HttpClient.CreateBranchAsync("DF Sevilla");

        (await fixture.FindUserIdByAccountAsync(branch.BusinessId, UserRows.AccountIdOf(memberTokens.AccessToken))).ShouldNotBeNull();
    }
}
