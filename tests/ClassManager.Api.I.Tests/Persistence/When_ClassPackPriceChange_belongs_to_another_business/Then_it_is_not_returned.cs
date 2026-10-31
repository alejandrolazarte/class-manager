using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClassPackPriceChange_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherPlan = await otherBusiness.HttpClient.CreateMonthlyPlanAsync();
        (await otherBusiness.HttpClient.PutPlanPriceAsync(otherPlan, 30000m, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClassPackPriceChanges.AnyAsync(change => change.ClassPackId == otherPlan.Id)).ShouldBeFalse();
    }
}
