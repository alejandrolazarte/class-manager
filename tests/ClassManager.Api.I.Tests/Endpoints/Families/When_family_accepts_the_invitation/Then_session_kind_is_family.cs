using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_accepts_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_session_kind_is_family(ApiFixture fixture)
{
    [Fact]
    public async Task Then_session_kind_is_family_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();

        scenario.Tokens.Kind.ShouldBe(AccountKinds.Family);
    }
}
