using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_records_attendance_in_their_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_204(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_204_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PutAttendanceAsync(
            scenario.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.CoachStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
