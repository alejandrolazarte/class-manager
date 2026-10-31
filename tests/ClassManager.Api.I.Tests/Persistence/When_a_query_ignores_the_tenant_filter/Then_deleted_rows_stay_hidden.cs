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
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();
        (await business.HttpClient.PutStudentPlanAsync(family.StudentId, null, effectiveFrom: FeeRequests.NextMonth)).EnsureSuccessStatusCode();
        (await business.HttpClient.DeleteStudentPlanChangeAsync(family.StudentId, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(otherBusiness.Business.Id);
        var visibleChanges = await context.StudentPlanChanges
            .IgnoreTenantFilter()
            .Where(change => change.StudentId == family.StudentId)
            .ToListAsync();

        visibleChanges.ShouldHaveSingleItem().IsDeleted.ShouldBeFalse();
    }
}
