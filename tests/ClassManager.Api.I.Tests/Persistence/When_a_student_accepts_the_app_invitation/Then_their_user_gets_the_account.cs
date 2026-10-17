using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_student_accepts_the_app_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_user_gets_the_account(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_user_gets_the_account_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        var clientAccount = await context.ClientAccounts.AsNoTracking().SingleAsync(account => account.ClientId == scenario.ClientId);
        var client = await context.Clients.AsNoTracking().SingleAsync(row => row.Id == scenario.ClientId);
        var user = await context.Users.AsNoTracking().SingleAsync(row => row.Id == clientAccount.UserId);

        user.AccountId.ShouldBe(UserRows.AccountIdOf(scenario.Tokens.AccessToken));
        clientAccount.UserId.ShouldBe(client.UserId);
    }
}
