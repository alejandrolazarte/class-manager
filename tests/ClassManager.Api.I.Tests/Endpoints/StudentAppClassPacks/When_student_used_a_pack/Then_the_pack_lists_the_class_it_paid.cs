using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassPacks.When_student_used_a_pack;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_pack_lists_the_class_it_paid(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_pack_lists_the_class_it_paid_Run()
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

        var classPacks = await scenario.Student.GetStudentAppClassPacksAsync();

        var purchase = classPacks!.Purchases.Single();
        purchase.Classes.Single().ShouldBe(new(lastWeek, "Clase semanal", studentId, IsPrivateLesson: false));
        purchase.RemainingClasses.ShouldBe(7);
    }
}
