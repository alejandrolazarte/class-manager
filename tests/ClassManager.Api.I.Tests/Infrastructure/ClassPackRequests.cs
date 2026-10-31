using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ClassPackRequests
{
    public const string PackName = "4 clases";
    public const string MonthlyPlanName = "Cuota mensual";
    public const int PrivateLessonMinutes = 45;

    public static Task<HttpResponseMessage> PostClassPackAsync(
        this HttpClient httpClient,
        string name = PackName,
        int classCount = 4,
        decimal price = 80m,
        int? validityMonths = 1,
        IReadOnlyList<long>? classGroupIds = null) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(
                name,
                classCount,
                price,
                validityMonths,
                classGroupIds is { Count: > 0 } ? null : PrivateLessonMinutes,
                classGroupIds),
            ApiRequests.JsonOptions);

    public static async Task<ClassPackResponse> CreateClassPackAsync(
        this HttpClient httpClient, string name = PackName, int classCount = 4, IReadOnlyList<long>? classGroupIds = null)
    {
        using var response = await httpClient.PostClassPackAsync(name, classCount, classGroupIds: classGroupIds);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<ClassPackResponse> CreateAnyGroupClassPackAsync(this HttpClient httpClient, string name = PackName, int classCount = 4)
    {
        using var response = await httpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(name, classCount, 80m, 1, CoversAnyGroupClass: true),
            ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<ClassPackResponse> CreateMonthlyPlanAsync(this HttpClient httpClient, string name = MonthlyPlanName, int? classCount = null)
    {
        using var response = await httpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(name, classCount, 25000m, null, CoversAnyGroupClass: true, RenewsMonthly: true),
            ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PutClassPackClassGroupsAsync(this HttpClient httpClient, ClassPackResponse classPack, IReadOnlyList<long> classGroupIds) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.ClassPacks}/{classPack.Id}",
            new UpdateClassPackRequest(classPack.Name, classPack.ClassCount, classPack.Price, classPack.ValidityMonths, ClassGroupIds: classGroupIds),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostClassPackSaleAsync(
        this HttpClient httpClient, long clientId, long classPackId, decimal? price = null, long? trialLessonId = null) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.ClassPackPurchasesSegment}",
            new SellClassPackRequest(classPackId, price, null, PaymentMethod.Cash, null, trialLessonId),
            ApiRequests.JsonOptions);

    public static async Task<ClassPackPurchaseResponse> SellClassPackAsync(this HttpClient httpClient, long clientId, long classPackId)
    {
        using var response = await httpClient.PostClassPackSaleAsync(clientId, classPackId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackPurchaseResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<ClassBalanceResponse?> GetClassBalanceAsync(this HttpClient httpClient, long clientId) =>
        httpClient.GetFromJsonAsync<ClassBalanceResponse>(
            new Uri($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.ClassBalance}", UriKind.Relative), ApiRequests.JsonOptions);
}
