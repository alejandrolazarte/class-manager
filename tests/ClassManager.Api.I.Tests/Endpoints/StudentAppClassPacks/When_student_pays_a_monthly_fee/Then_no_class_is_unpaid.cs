using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassPacks.When_student_pays_a_monthly_fee;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_class_is_unpaid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_class_is_unpaid_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var lastWeek = InstructorScenario.ClassDate.AddDays(-7);
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(lastWeek);
        await scenario.Instructors.Business.HttpClient.GiveMonthlyPlanAsync(studentId);
        (await owner.PutAttendanceAsync(weekly.Id, lastWeek, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var classPacks = await scenario.Student.GetStudentAppClassPacksAsync();

        classPacks!.UnpaidClasses.ShouldBeEmpty();
    }
}
