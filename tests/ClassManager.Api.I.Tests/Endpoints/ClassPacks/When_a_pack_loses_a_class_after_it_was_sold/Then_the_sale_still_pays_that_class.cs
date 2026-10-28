using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_a_pack_loses_a_class_after_it_was_sold;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_sale_still_pays_that_class(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_sale_still_pays_that_class_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var classGroup = await owner.CreateClassGroupWithInstructorAsync();
        var otherClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(classGroup.InstructorId, [DayOfWeek.Monday], "20:00") with { Name = "Aquagym" });
        var client = await owner.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await owner.EnrollAsync(classGroup.Id, studentId);
        (await owner.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await owner.CreateClassPackAsync(classGroupIds: [classGroup.Id]);
        await owner.SellClassPackAsync(client.Id, classPack.Id);
        (await owner.PutClassPackClassGroupsAsync(classPack, [otherClassGroup.Id])).EnsureSuccessStatusCode();

        (await owner.PutAttendanceAsync(classGroup.Id, EnrollmentRequests.Today, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        (await owner.GetClassBalanceAsync(client.Id))!.AvailableClasses.ShouldBe(3);
    }
}
