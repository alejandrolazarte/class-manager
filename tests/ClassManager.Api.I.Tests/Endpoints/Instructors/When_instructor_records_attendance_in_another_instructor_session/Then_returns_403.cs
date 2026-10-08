using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_records_attendance_in_another_instructor_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PutAttendanceAsync(
            scenario.OtherClassGroup.Id, InstructorScenario.ClassDate, scenario.OtherStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
