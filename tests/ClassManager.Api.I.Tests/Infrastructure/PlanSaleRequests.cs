using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class PlanSaleRequests
{
    public static Task<HttpResponseMessage> PostPlanSaleAsync(
        this HttpClient httpClient, long studentId, long? classPackId, decimal? paidAmount = FeeRequests.DefaultFee) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Students}/{studentId}{ApiRoutes.StudentPlanSales}",
            new SellStudentPlanRequest(classPackId, paidAmount, PaymentMethod.Cash, null),
            ApiRequests.JsonOptions);

    public static async Task<long> RegisterEnrolledStudentAsync(this HttpClient httpClient)
    {
        var classGroup = await httpClient.CreateClassGroupWithInstructorAsync();
        var client = await httpClient.RegisterClientAsync(students: [new Core.UseCases.Students.NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await httpClient.EnrollAsync(classGroup.Id, studentId);
        return studentId;
    }
}
