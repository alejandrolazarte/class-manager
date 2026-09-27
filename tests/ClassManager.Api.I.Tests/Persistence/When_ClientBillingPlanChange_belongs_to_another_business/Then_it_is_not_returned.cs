using ClassManager.Core.Domain.Fees;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClientBillingPlanChange_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClient = await otherBusiness.HttpClient.RegisterClientAsync();
        (await otherBusiness.HttpClient.PutBillingPlanAsync(otherClient.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClientBillingPlanChanges.AnyAsync(change => change.ClientId == otherClient.Id)).ShouldBeFalse();
    }
}
