using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_student_warns_after_attendance_was_taken;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classGroupId = scenario.Coaches.CoachClassGroup.Id;
        var studentId = scenario.Coaches.CoachStudentId;
        (await scenario.Coaches.Coach.PutAttendanceAsync(classGroupId, CoachScenario.ClassDate, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        using var response = await scenario.Student.PutAbsenceAsync(studentId, classGroupId, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.AttendanceTaken);
    }
}
