using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_had_a_private_lesson;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_with_its_attendance(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_with_its_attendance_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var lesson = await owner.SchedulePrivateLessonAsync(scenario.Instructors.InstructorId, studentId, InstructorScenario.ClassDate.AddDays(-2));
        (await owner.PutPrivateLessonAttendanceAsync(lesson.Id, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var history = await scenario.Student.GetClassHistoryAsync(studentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Single().ShouldSatisfyAllConditions(
            pastClass => pastClass.IsPrivateLesson.ShouldBeTrue(),
            pastClass => pastClass.Status.ShouldBe(PastClassStatus.Attended));
    }
}
