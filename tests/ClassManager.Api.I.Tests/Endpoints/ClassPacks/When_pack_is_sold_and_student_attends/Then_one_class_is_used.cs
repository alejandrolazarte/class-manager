using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_pack_is_sold_and_student_attends;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_one_class_is_used(ApiFixture fixture)
{
    [Fact]
    public async Task Then_one_class_is_used_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await business.HttpClient.EnrollAsync(classGroup.Id, studentId);
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        await business.HttpClient.SellClassPackAsync(client.Id, classPack.Id);

        (await business.HttpClient.PutAttendanceAsync(classGroup.Id, EnrollmentRequests.Today, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        var balance = await business.HttpClient.GetClassBalanceAsync(client.Id);
        balance!.AvailableClasses.ShouldBe(3);
        balance.Purchases.Single().Status.ShouldBe(ClassPackPurchaseStatus.Active);
        (await business.HttpClient.GetOrderSummaryAsync()).ClassPackSales.ShouldBe(80m);
    }
}
