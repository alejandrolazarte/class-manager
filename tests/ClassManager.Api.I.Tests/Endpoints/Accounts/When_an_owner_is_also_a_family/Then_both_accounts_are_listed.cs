using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_is_also_a_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_both_accounts_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_both_accounts_are_listed_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoFamilyAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        var accounts = await client.ListAccountsAsync();

        accounts.ShouldBe(
        [
            new AccountResponse(seeded.OwnBusinessId, seeded.OwnBusinessName, AccountKinds.Team, IsCurrent: true),
            new AccountResponse(seeded.Family.Coaches.Business.Business.Id, seeded.Family.Coaches.Business.Business.Name, AccountKinds.Family, IsCurrent: false),
        ], ignoreOrder: true);
    }
}
