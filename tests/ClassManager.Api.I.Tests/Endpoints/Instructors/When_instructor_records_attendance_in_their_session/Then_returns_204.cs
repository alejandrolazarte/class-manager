using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_records_attendance_in_their_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_204(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_204_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PutAttendanceAsync(
            scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
