using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_records_attendance_in_another_coach_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PutAttendanceAsync(
            scenario.OtherClassGroup.Id, CoachScenario.ClassDate, scenario.OtherStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
