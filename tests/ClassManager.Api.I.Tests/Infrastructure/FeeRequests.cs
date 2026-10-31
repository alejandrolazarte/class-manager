using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class FeeRequests
{
    public const decimal DefaultFee = 12000m;
    public const string CurrentMonth = "2026-09";
    public const string NextMonth = "2026-10";

    public static Task<HttpResponseMessage> PutStudentPlanAsync(
        this HttpClient httpClient, long studentId, long? classPackId, decimal? customPrice = null, string? effectiveFrom = null) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.Students}/{studentId}{ApiRoutes.StudentPlan}",
            new SetStudentPlanRequest(classPackId, customPrice, effectiveFrom),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteStudentPlanChangeAsync(this HttpClient httpClient, long studentId, string month) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Students}/{studentId}{ApiRoutes.StudentPlan}/{month}", UriKind.Relative));

    public static async Task<long> GiveMonthlyPlanAsync(this HttpClient httpClient, long studentId, decimal price = DefaultFee)
    {
        var plan = await httpClient.CreateMonthlyPlanAsync($"Cuota {Guid.NewGuid():N}"[..20], price: price);
        (await httpClient.PutStudentPlanAsync(studentId, plan.Id)).EnsureSuccessStatusCode();
        return plan.Id;
    }

    public static async Task<MonthlyPlanClient> EnrollClientWithMonthlyPlanAsync(this HttpClient httpClient, decimal price = DefaultFee)
    {
        var classGroup = await httpClient.CreateClassGroupWithInstructorAsync();
        var client = await httpClient.RegisterClientAsync(students: [new Core.UseCases.Students.NewStudent(ApiRequests.StudentFullName, null, null)]);
        var studentId = client.Students[0].Id;
        await httpClient.EnrollAsync(classGroup.Id, studentId);
        var planId = await httpClient.GiveMonthlyPlanAsync(studentId, price);
        return new MonthlyPlanClient(client.Id, studentId, planId);
    }

    public static Task<HttpResponseMessage> PostPaymentAsync(this HttpClient httpClient, long clientId, decimal amount = DefaultFee) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.PaymentsSegment}",
            new RecordPaymentRequest(amount, CurrentMonth, null, PaymentMethod.Transfer, null),
            ApiRequests.JsonOptions);

    public static Task<MonthlyFeesResponse?> GetMonthlyFeesAsync(this HttpClient httpClient, string month = CurrentMonth) =>
        httpClient.GetFromJsonAsync<MonthlyFeesResponse>(
            new Uri($"{ApiRoutes.Fees}?month={month}", UriKind.Relative), ApiRequests.JsonOptions);

    public static long ClientIdOf(this MonthlyFeesResponse fees, string studentFullName) =>
        fees.Clients.Select(client => (client.ClientId, client.StudentNames))
            .Concat(fees.ClassPackClients.Select(client => (client.ClientId, client.StudentNames)))
            .Single(client => client.StudentNames.Contains(studentFullName))
            .ClientId;

    public static async Task<long> EnrollClientAsync(this HttpClient httpClient)
    {
        var classGroup = await httpClient.CreateClassGroupWithInstructorAsync();
        var client = await httpClient.RegisterClientAsync(students: [new Core.UseCases.Students.NewStudent(ApiRequests.StudentFullName, null, null)]);
        await httpClient.EnrollAsync(classGroup.Id, client.Students[0].Id);
        return client.Id;
    }
}

public sealed record MonthlyPlanClient(long ClientId, long StudentId, long PlanId);
