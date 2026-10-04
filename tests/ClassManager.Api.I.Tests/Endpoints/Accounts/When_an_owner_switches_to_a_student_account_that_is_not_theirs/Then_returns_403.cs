using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_switches_to_a_student_account_that_is_not_theirs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoStudentAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        var otherBusiness = await fixture.SeedBusinessAsync();

        using var response = await client.PostSwitchAccountAsync(seeded.OwnerTokens.RefreshToken, otherBusiness.Business.Id, AccountKinds.Student);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
