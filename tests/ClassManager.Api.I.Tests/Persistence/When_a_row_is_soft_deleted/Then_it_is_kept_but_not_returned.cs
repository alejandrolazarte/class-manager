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
        var family = await business.HttpClient.EnrollClientWithMonthlyPlanAsync();
        (await business.HttpClient.PutStudentPlanAsync(family.StudentId, null, effectiveFrom: FeeRequests.NextMonth)).EnsureSuccessStatusCode();
        (await business.HttpClient.DeleteStudentPlanChangeAsync(family.StudentId, FeeRequests.NextMonth)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.StudentPlanChanges.AnyAsync(change => change.StudentId == family.StudentId && change.EffectiveFrom > new DateOnly(2026, 9, 30))).ShouldBeFalse();
        (await context.StudentPlanChanges
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(change => change.StudentId == family.StudentId && change.IsDeleted))
            .DeletedAt.ShouldNotBeNull();
    }
}
