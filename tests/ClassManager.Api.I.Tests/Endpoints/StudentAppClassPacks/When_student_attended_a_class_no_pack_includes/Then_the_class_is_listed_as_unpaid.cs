using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassPacks.When_student_attended_a_class_no_pack_includes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_listed_as_unpaid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_listed_as_unpaid_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var studentId = scenario.Instructors.InstructorStudentId;
        var lastWeek = InstructorScenario.ClassDate.AddDays(-7);
        var weekly = await scenario.EnrollInWeeklyClassSinceAsync(lastWeek);
        var otherClass = await scenario.CreateOtherClassAsync();
        (await owner.PutBillingPlanAsync(scenario.ClientId, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await owner.CreateClassPackAsync("Pack 8 clases", classCount: 8, classGroupIds: [otherClass.Id]);
        await owner.SellClassPackAsync(scenario.ClientId, classPack.Id);
        (await owner.PutAttendanceAsync(weekly.Id, lastWeek, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var classPacks = await scenario.Student.GetStudentAppClassPacksAsync();

        classPacks!.UnpaidClasses.Single().ShouldBe(new(lastWeek, "Clase semanal", studentId, IsPrivateLesson: false));
    }
}
