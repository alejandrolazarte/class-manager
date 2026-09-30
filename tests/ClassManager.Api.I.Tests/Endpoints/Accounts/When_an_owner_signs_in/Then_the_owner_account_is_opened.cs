using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_signs_in;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_owner_account_is_opened(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_owner_account_is_opened_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoFamilyAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        seeded.OwnerTokens.Kind.ShouldBe(AccountKinds.Team);
        (await client.ListBranchesAsync()).Single().BusinessId.ShouldBe(seeded.OwnBusinessId);
    }
}
