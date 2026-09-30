using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_owner_switches_to_the_family_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_family_session_is_issued(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_family_session_is_issued_Run()
    {
        var seeded = await fixture.SeedOwnerWhoIsAlsoFamilyAsync();
        using var client = fixture.CreateClientWithToken(seeded.OwnerTokens.AccessToken);

        using var response = await client.PostSwitchAccountAsync(
            seeded.OwnerTokens.RefreshToken, seeded.Family.Coaches.Business.Business.Id, AccountKinds.Family);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = await response.ReadTokensAsync();
        tokens.Kind.ShouldBe(AccountKinds.Family);
        using var familyClient = fixture.CreateClientWithToken(tokens.AccessToken);
        (await familyClient.GetFamilyHomeAsync())!.Students.ShouldHaveSingleItem().FullName.ShouldBe(CoachScenario.CoachStudentFullName);
    }
}
