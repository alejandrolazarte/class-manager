using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_is_also_a_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_both_accounts_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_both_accounts_are_listed_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoStudentAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        var accounts = await client.ListAccountsAsync();

        accounts.ShouldBe(
        [
            new AccountResponse(seeded.OwnBusinessId, seeded.OwnBusinessName, AccountKinds.Team, IsCurrent: true),
            new AccountResponse(seeded.Student.Coaches.Business.Business.Id, seeded.Student.Coaches.Business.Business.Name, AccountKinds.Student, IsCurrent: false),
        ], ignoreOrder: true);
    }
}
