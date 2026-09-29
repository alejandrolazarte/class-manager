using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Orders;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Orders.When_refunding_a_pack_with_one_class_used;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_unused_classes_are_refunded(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_unused_classes_are_refunded_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await business.HttpClient.EnrollAsync(classGroup.Id, studentId);
        (await business.HttpClient.PutBillingPlanAsync(client.Id, BillingPlanKind.ClassPacks)).EnsureSuccessStatusCode();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        var order = await business.HttpClient.SellAtCounterAsync(client.Id, [OrderRequests.PackLine(classPack.Id)]);
        (await business.HttpClient.PutAttendanceAsync(classGroup.Id, EnrollmentRequests.Today, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.PostRefundAsync(order.Id, new RefundLine(order.Lines[0].Id, null, false));

        (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!.RefundedAmount.ShouldBe(60m);
        var balance = await business.HttpClient.GetClassBalanceAsync(client.Id);
        balance!.Purchases.Single().ClassCount.ShouldBe(1);
        balance.AvailableClasses.ShouldBe(0);
    }
}
