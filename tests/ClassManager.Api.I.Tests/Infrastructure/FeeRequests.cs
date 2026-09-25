using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class FeeRequests
{
    public const decimal DefaultFee = 12000m;
    public const string CurrentMonth = "2026-09";

    public static async Task SetDefaultFeeAsync(this HttpClient httpClient, decimal? amount = DefaultFee)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"{ApiRoutes.Business}{ApiRoutes.MonthlyFee}", new SetMonthlyFeeRequest(amount), ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
    }

    public static Task<HttpResponseMessage> PostPaymentAsync(this HttpClient httpClient, Guid clientId, decimal amount = DefaultFee) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}",
            new RecordPaymentRequest(amount, CurrentMonth, null, PaymentMethod.Transfer, null),
            ApiRequests.JsonOptions);

    public static Task<MonthlyFeesResponse?> GetMonthlyFeesAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<MonthlyFeesResponse>(
            new Uri($"{ApiRoutes.Fees}?month={CurrentMonth}", UriKind.Relative), ApiRequests.JsonOptions);

    public static async Task<Guid> EnrollClientAsync(this HttpClient httpClient)
    {
        var classGroup = await httpClient.CreateClassGroupWithInstructorAsync();
        var client = await httpClient.RegisterClientAsync(students: [new Core.UseCases.Students.NewStudent(ApiRequests.StudentFullName, null, null)]);
        await httpClient.EnrollAsync(classGroup.Id, client.Students[0].Id);
        return client.Id;
    }
}
