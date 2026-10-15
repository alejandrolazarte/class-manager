using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_the_last_used_branch_is_no_longer_available;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_tokens_are_for_a_branch_they_still_belong_to(ApiFixture fixture)
{
    [Fact]
    public async Task Then_tokens_are_for_a_branch_they_still_belong_to_Run()
    {
        var otherBusiness = await fixture.SeedBusinessAsync();
        var email = AuthenticationRequests.UniqueEmail();
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand(email));
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);
        var ownBusinessId = (await ownerClient.GetCurrentMemberAsync()).BusinessId;
        await otherBusiness.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using (var acceptResponse = await client.PostAcceptInvitationAsync(
            fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email), password: string.Empty))
        {
            acceptResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        await client.SwitchBranchAsync(tokens.RefreshToken, otherBusiness.Business.Id);
        var invitedMember = (await otherBusiness.HttpClient.GetTeamAsync()).Members.Single(member => member.Email == email);
        using (var removeResponse = await otherBusiness.HttpClient.DeleteMemberAsync(invitedMember.Id))
        {
            removeResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        var signInTokens = await client.SignInAsync(email);

        using var signedInClient = fixture.CreateClientWithToken(signInTokens.AccessToken);
        (await signedInClient.GetCurrentMemberAsync()).BusinessId.ShouldBe(ownBusinessId);
    }
}
