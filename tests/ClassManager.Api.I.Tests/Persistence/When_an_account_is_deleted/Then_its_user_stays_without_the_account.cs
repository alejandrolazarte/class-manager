using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_an_account_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_user_stays_without_the_account(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_user_stays_without_the_account_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var businessId = scenario.Instructors.Business.Business.Id;
        var userId = UserRows.UserIdOf(scenario.Tokens.AccessToken);
        await using var context = fixture.CreateDbContext(businessId);
        var fullName = await context.Users.Where(user => user.Id == userId).Select(user => user.FullName).SingleAsync();

        await scenario.DeleteAccountAsync();

        var user = await context.Users.AsNoTracking().SingleAsync(user => user.Id == userId);
        user.AccountId.ShouldBeNull();
        user.FullName.ShouldBe(fullName);
        var appAccess = await context.ClientAccounts
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .AsNoTracking()
            .SingleAsync(account => account.UserId == userId);
        appAccess.IsDeleted.ShouldBeTrue();
        appAccess.DeletedByUserId.ShouldBe(userId);
    }
}
