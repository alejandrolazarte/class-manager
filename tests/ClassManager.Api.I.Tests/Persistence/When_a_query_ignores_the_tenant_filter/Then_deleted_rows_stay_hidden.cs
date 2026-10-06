using ClassManager.Core.Domain.Fees;
using ClassManager.Tenancy.AspNetCore.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_query_ignores_the_tenant_filter;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_deleted_rows_stay_hidden(ApiFixture fixture)
{
    [Fact]
    public async Task Then_deleted_rows_stay_hidden_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks, effectiveFrom: FeeRequests.NextMonth)).EnsureSuccessStatusCode();
        (await business.HttpClient.DeleteBillingPlanChangeAsync(client.Id, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(otherBusiness.Business.Id);
        var visibleChanges = await context.ClientBillingPlanChanges
            .IgnoreTenantFilter()
            .Where(change => change.ClientId == client.Id)
            .ToListAsync();

        visibleChanges.ShouldHaveSingleItem().IsDeleted.ShouldBeFalse();
    }
}
