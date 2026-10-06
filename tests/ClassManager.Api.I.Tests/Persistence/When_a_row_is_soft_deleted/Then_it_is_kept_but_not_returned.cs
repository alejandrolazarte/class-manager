using ClassManager.Core.Domain.Fees;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_row_is_soft_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_but_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_but_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks, effectiveFrom: FeeRequests.NextMonth)).EnsureSuccessStatusCode();
        (await business.HttpClient.DeleteBillingPlanChangeAsync(client.Id, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClientBillingPlanChanges.AnyAsync(change => change.ClientId == client.Id)).ShouldBeFalse();
        (await context.ClientBillingPlanChanges
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(change => change.ClientId == client.Id))
            .DeletedOn.ShouldNotBeNull();
    }
}
