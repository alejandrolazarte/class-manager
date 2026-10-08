using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_student_warns_after_attendance_was_taken;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classGroupId = scenario.Instructors.InstructorClassGroup.Id;
        var studentId = scenario.Instructors.InstructorStudentId;
        (await scenario.Instructors.Instructor.PutAttendanceAsync(classGroupId, InstructorScenario.ClassDate, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        using var response = await scenario.Student.PutAbsenceAsync(studentId, classGroupId, InstructorScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.AttendanceTaken);
    }
}
