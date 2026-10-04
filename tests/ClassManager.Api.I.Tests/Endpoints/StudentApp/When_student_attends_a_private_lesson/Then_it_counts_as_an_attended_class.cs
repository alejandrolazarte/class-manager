using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_attends_a_private_lesson;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_counts_as_an_attended_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_counts_as_an_attended_class_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var lesson = await owner.SchedulePrivateLessonAsync(
            scenario.Coaches.CoachInstructorId, scenario.Coaches.CoachStudentId, CoachScenario.ClassDate);
        using (var response = await owner.PutPrivateLessonAttendanceAsync(lesson.Id, scenario.Coaches.CoachStudentId, AttendanceStatus.Present))
        {
            response.IsSuccessStatusCode.ShouldBeTrue();
        }

        var home = await scenario.Student.GetStudentAppHomeAsync();

        home!.Students.Single().Attendance.AttendedClasses.ShouldBe(1);
    }
}
