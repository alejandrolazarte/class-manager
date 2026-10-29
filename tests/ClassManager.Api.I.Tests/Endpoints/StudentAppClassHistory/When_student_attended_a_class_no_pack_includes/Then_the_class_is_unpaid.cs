using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_attended_a_class_no_pack_includes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_unpaid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_unpaid_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var lastWeek = InstructorScenario.ClassDate.AddDays(-7);
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(lastWeek);
        (await owner.PutBillingPlanAsync(scenario.ClientId, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        (await owner.PutAttendanceAsync(weekly.Id, lastWeek, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var history = await scenario.Student.GetClassHistoryAsync(studentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Single().IsUnpaid.ShouldBeTrue();
    }
}
