using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_signs_in;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_open_their_family(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_open_their_family_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        var tokens = await anonymous.SignInAsync(scenario.Email, FamilyRequests.FamilyPassword);

        tokens.Kind.ShouldBe(AccountKinds.Family);
        using var family = fixture.CreateClientWithToken(tokens.AccessToken);
        (await family.GetFamilyHomeAsync())!.ClientFullName.ShouldNotBeEmpty();
    }
}
