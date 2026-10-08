using ClassManager.Core.Domain.Businesses;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_team_member_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_deleted_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var instructorMember = (await scenario.Business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Instructor);

        (await scenario.Business.HttpClient.DeleteMemberAsync(instructorMember.Id)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(scenario.Business.Business.Id);
        (await context.BusinessMembers.AnyAsync(row => row.Id == instructorMember.Id)).ShouldBeFalse();
        (await context.BusinessMembers
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.Id == instructorMember.Id))
            .DeletedOn.ShouldBe(BusinessApiFactory.Now);
    }
}
