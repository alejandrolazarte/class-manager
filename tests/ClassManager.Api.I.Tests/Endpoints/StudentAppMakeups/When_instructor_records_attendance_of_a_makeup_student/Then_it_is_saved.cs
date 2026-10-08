using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_instructor_records_attendance_of_a_makeup_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_saved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_saved_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        using var response = await scenario.Instructors.Business.HttpClient.PutAttendanceAsync(
            scenario.Instructors.OtherClassGroup.Id, InstructorScenario.ClassDate, scenario.Instructors.InstructorStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
