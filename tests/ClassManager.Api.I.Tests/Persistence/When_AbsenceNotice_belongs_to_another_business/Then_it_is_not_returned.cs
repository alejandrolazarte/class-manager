using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_AbsenceNotice_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherFamily = await fixture.SeedFamilyScenarioAsync();
        (await otherFamily.Family.PutAbsenceAsync(
            otherFamily.Coaches.CoachStudentId, otherFamily.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.AbsenceNotices.AnyAsync(notice => notice.StudentId == otherFamily.Coaches.CoachStudentId)).ShouldBeFalse();
    }
}
