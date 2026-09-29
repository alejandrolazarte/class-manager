using ClassManager.Core.Abstractions.Security;

using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Families.When_team_token_calls_a_family_endpoint;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var familyUserId = (await ClientAccountUserIdAsync(fixture, scenario)).GetValueOrDefault();
        using var teamKindClient = fixture.CreateClientWithToken(
            fixture.CreateAccessToken(scenario.Coaches.Business.Business.Id, userId: familyUserId, kind: AccountKinds.Team));

        using var response = await teamKindClient.GetAsync(new Uri(ApiRoutes.Family, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private static async Task<Guid?> ClientAccountUserIdAsync(ApiFixture fixture, FamilyScenario scenario)
    {
        await using var context = fixture.CreateDbContext(scenario.Coaches.Business.Business.Id);
        return await context.ClientAccounts.Where(account => account.ClientId == scenario.FamilyId).Select(account => (Guid?)account.UserId).FirstOrDefaultAsync();
    }
}
