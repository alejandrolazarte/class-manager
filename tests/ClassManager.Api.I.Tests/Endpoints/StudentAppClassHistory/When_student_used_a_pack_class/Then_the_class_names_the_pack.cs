using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_student_used_a_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_names_the_pack(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_names_the_pack_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var lastWeek = InstructorScenario.ClassDate.AddDays(-7);
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(lastWeek);
        (await owner.PutBillingPlanAsync(scenario.ClientId, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await owner.CreateClassPackAsync("Pack 8 clases", classCount: 8, classGroupIds: [weekly.Id]);
        await owner.SellClassPackAsync(scenario.ClientId, classPack.Id);
        (await owner.PutAttendanceAsync(weekly.Id, lastWeek, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var history = await scenario.Student.GetClassHistoryAsync(studentId, ClassHistoryRequests.CurrentMonth);

        history!.Classes.Single().PaidWithPack!.Name.ShouldBe("Pack 8 clases");
    }
}
