using ClassManager.Core.Abstractions.Security;

using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_team_token_calls_a_student_endpoint;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var studentAccountId = (await ClientAccountIdAsync(fixture, scenario)).GetValueOrDefault();
        using var teamKindClient = fixture.CreateClientWithToken(
            fixture.CreateAccessToken(scenario.Instructors.Business.Business.Id, accountId: studentAccountId, kind: AccountKinds.Team));

        using var response = await teamKindClient.GetAsync(new Uri(ApiRoutes.StudentApp, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    private static async Task<Guid?> ClientAccountIdAsync(ApiFixture fixture, StudentAppScenario scenario)
    {
        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        return await context.ClientAccounts.Where(account => account.ClientId == scenario.ClientId).Select(account => account.User.AccountId).FirstOrDefaultAsync();
    }
}
