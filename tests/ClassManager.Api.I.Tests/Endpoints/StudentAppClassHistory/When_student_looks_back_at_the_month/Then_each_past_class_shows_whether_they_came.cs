using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_looks_back_at_the_month;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_past_class_shows_whether_they_came(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_past_class_shows_whether_they_came_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var today = InstructorScenario.ClassDate;
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(today.AddDays(-21));
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-21), studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();
        (await owner.PutAttendanceAsync(weekly.Id, today.AddDays(-14), studentId, AttendanceStatus.Absent)).EnsureSuccessStatusCode();

        var history = await scenario.Student.GetClassHistoryAsync(studentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Select(pastClass => pastClass.Status)
            .ShouldBe([PastClassStatus.Attended, PastClassStatus.Absent, PastClassStatus.Unmarked]);
    }
}
