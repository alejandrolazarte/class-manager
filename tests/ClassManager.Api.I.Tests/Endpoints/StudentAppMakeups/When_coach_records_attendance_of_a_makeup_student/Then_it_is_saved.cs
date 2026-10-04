using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_coach_records_attendance_of_a_makeup_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_saved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_saved_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.NoticeAndBookOtherClassAsync();

        using var response = await scenario.Coaches.Business.HttpClient.PutAttendanceAsync(
            scenario.Coaches.OtherClassGroup.Id, CoachScenario.ClassDate, scenario.Coaches.CoachStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
