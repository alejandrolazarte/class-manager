using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_substitutes_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_record_attendance(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_record_attendance_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        using var assignment = await scenario.Business.HttpClient.PutSubstituteAsync(
            scenario.OtherClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorId);

        using var response = await scenario.Instructor.PutAttendanceAsync(
            scenario.OtherClassGroup.Id, InstructorScenario.ClassDate, scenario.OtherStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
