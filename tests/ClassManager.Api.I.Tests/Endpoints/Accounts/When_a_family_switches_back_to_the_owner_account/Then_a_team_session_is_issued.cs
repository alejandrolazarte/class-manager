using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_family_switches_back_to_the_owner_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_team_session_is_issued(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_team_session_is_issued_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoFamilyAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        var familyTokens = await (await client.PostSwitchAccountAsync(
            seeded.OwnerTokens.RefreshToken, seeded.Family.Coaches.Business.Business.Id, AccountKinds.Family)).ReadTokensAsync();
        using var familyClient = fixture.CreateClientWithToken(familyTokens.AccessToken);

        using var response = await familyClient.PostSwitchAccountAsync(familyTokens.RefreshToken, seeded.OwnBusinessId, AccountKinds.Team);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.ReadTokensAsync()).Kind.ShouldBe(AccountKinds.Team);
    }
}
