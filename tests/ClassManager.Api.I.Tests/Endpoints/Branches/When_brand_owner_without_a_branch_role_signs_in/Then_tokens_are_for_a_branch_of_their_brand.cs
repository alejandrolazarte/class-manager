using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_without_a_branch_role_signs_in;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_tokens_are_for_a_branch_of_their_brand(ApiFixture fixture)
{
    [Fact]
    public async Task Then_tokens_are_for_a_branch_of_their_brand_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailSender.InvitationTokenSentTo(email));
        var invitedMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Email == email);
        using (var promoteResponse = await business.HttpClient.PutBrandOwnerAsync(invitedMember.Id))
        {
            promoteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        using (var removeResponse = await business.HttpClient.DeleteMemberAsync(invitedMember.Id))
        {
            removeResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        var tokens = await anonymousClient.SignInAsync(email, MemberRequests.InviteePassword);

        using var brandOwnerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var member = await brandOwnerClient.GetCurrentMemberAsync();
        member.BusinessId.ShouldBe(business.Business.Id);
        member.IsBrandOwner.ShouldBeTrue();
        member.BranchRole.ShouldBeNull();
    }
}
