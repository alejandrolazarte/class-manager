using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_substitutes_another_coach;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_record_attendance(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_record_attendance_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        using var assignment = await scenario.Business.HttpClient.PutSubstituteAsync(
            scenario.OtherClassGroup.Id, CoachScenario.ClassDate, scenario.CoachInstructorId);

        using var response = await scenario.Coach.PutAttendanceAsync(
            scenario.OtherClassGroup.Id, CoachScenario.ClassDate, scenario.OtherStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
