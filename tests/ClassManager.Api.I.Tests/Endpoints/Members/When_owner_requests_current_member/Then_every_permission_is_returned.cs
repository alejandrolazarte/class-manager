using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_requests_current_member;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_permission_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_permission_is_returned_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var tokens = await client.SignUpAsync(AuthenticationRequests.SignUpCommand());
        using var ownerClient = fixture.CreateClientWithToken(tokens.AccessToken);

        var member = await ownerClient.GetCurrentMemberAsync();

        member.Permissions.ShouldBe(Permissions.All, ignoreOrder: true);
        member.IsBrandOwner.ShouldBeTrue();
        member.BranchRole.ShouldBe(BusinessRole.BranchOwner);
    }
}
