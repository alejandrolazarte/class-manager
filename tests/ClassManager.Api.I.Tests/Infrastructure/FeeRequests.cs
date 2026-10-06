using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Businesses;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class FeeRequests
{
    public const decimal DefaultFee = 12000m;
    public const string CurrentMonth = "2026-09";
    public const string NextMonth = "2026-10";

    public static async Task<BusinessResponse> SetDefaultFeeAsync(this HttpClient httpClient, decimal? amount = DefaultFee, string? effectiveFrom = null)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"{ApiRoutes.Business}{ApiRoutes.MonthlyFee}", new SetMonthlyFeeRequest(amount, effectiveFrom), ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<BusinessResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PutBillingPlanAsync(
        this HttpClient httpClient, Guid clientId, BillingPlanKind kind, decimal? customFee = null, string? effectiveFrom = null) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.BillingPlan}",
            new SetClientBillingPlanRequest(kind, customFee, effectiveFrom),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteBillingPlanChangeAsync(this HttpClient httpClient, Guid clientId, string month) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.BillingPlan}/{month}", UriKind.Relative));

    public static Task<HttpResponseMessage> DeleteDefaultFeeChangeAsync(this HttpClient httpClient, string month) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Business}{ApiRoutes.MonthlyFee}/{month}", UriKind.Relative));

    public static Task<HttpResponseMessage> PostPaymentAsync(this HttpClient httpClient, Guid clientId, decimal amount = DefaultFee) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}",
            new RecordPaymentRequest(amount, CurrentMonth, null, PaymentMethod.Transfer, null),
            ApiRequests.JsonOptions);

    public static Task<MonthlyFeesResponse?> GetMonthlyFeesAsync(this HttpClient httpClient, string month = CurrentMonth) =>
        httpClient.GetFromJsonAsync<MonthlyFeesResponse>(
            new Uri($"{ApiRoutes.Fees}?month={month}", UriKind.Relative), ApiRequests.JsonOptions);

    public static async Task<Guid> EnrollClientAsync(this HttpClient httpClient)
    {
        var classGroup = await httpClient.CreateClassGroupWithInstructorAsync();
        var client = await httpClient.RegisterClientAsync(students: [new Core.UseCases.Students.NewStudent(ApiRequests.StudentFullName, null, null)]);
        await httpClient.EnrollAsync(classGroup.Id, client.Students[0].Id);
        return client.Id;
    }
}
