using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_class_comment_is_cleared;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_deleted_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        var owner = scenario.Business.HttpClient;
        (await owner.PutFeedbackAsync(scenario.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.CoachStudentId, "Muy bien"))
            .EnsureSuccessStatusCode();

        (await owner.PutFeedbackAsync(scenario.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.CoachStudentId, " "))
            .EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(scenario.Business.Business.Id);
        (await context.ClassFeedbacks.AnyAsync(row => row.StudentId == scenario.CoachStudentId)).ShouldBeFalse();
        (await context.ClassFeedbacks
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.StudentId == scenario.CoachStudentId))
            .DeletedOn.ShouldBe(BusinessApiFactory.Now);
    }
}
