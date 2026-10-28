using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_the_pack_does_not_cover_the_class_attended;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_owed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_owed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var attendedClassGroup = await owner.CreateClassGroupWithInstructorAsync();
        var otherClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(attendedClassGroup.InstructorId, [DayOfWeek.Monday], "20:00") with { Name = "Aquagym" });
        var client = await owner.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await owner.EnrollAsync(attendedClassGroup.Id, studentId);
        (await owner.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await owner.CreateClassPackAsync(classGroupIds: [otherClassGroup.Id]);
        await owner.SellClassPackAsync(client.Id, classPack.Id);

        (await owner.PutAttendanceAsync(attendedClassGroup.Id, EnrollmentRequests.Today, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var balance = await owner.GetClassBalanceAsync(client.Id);
        balance!.ShouldSatisfyAllConditions(
            clientBalance => clientBalance.UnpaidClasses.ShouldBe(1),
            clientBalance => clientBalance.AvailableClasses.ShouldBe(4));
    }
}
