using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyPackClasses.When_booked_student_misses_the_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_class_is_used(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_class_is_used_Run()
    {
        var scenario = await fixture.SeedPackFamilyScenarioAsync();
        await scenario.BookPackClassAsync();

        using var response = await scenario.Owner.PutAttendanceAsync(
            scenario.PackClassGroupId, CoachScenario.ClassDate, scenario.StudentId, AttendanceStatus.Absent);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await scenario.Owner.GetClassBalanceAsync(scenario.Scenario.FamilyId))!.AvailableClasses.ShouldBe(3);
    }
}
