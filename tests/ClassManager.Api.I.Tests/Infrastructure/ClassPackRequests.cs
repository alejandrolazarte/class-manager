using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ClassPackRequests
{
    public const string PackName = "4 clases";

    public static Task<HttpResponseMessage> PostClassPackAsync(
        this HttpClient httpClient,
        string name = PackName,
        int classCount = 4,
        decimal price = 80m,
        int? validityMonths = 1,
        IReadOnlyList<Guid>? classGroupIds = null) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(name, classCount, price, validityMonths, ClassGroupIds: classGroupIds),
            ApiRequests.JsonOptions);

    public static async Task<ClassPackResponse> CreateClassPackAsync(
        this HttpClient httpClient, string name = PackName, int classCount = 4, IReadOnlyList<Guid>? classGroupIds = null)
    {
        using var response = await httpClient.PostClassPackAsync(name, classCount, classGroupIds: classGroupIds);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostClassPackSaleAsync(
        this HttpClient httpClient, Guid clientId, Guid classPackId, decimal? price = null, Guid? trialLessonId = null) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.ClassPackPurchasesSegment}",
            new SellClassPackRequest(classPackId, price, null, PaymentMethod.Cash, null, trialLessonId),
            ApiRequests.JsonOptions);

    public static async Task<ClassPackPurchaseResponse> SellClassPackAsync(this HttpClient httpClient, Guid clientId, Guid classPackId)
    {
        using var response = await httpClient.PostClassPackSaleAsync(clientId, classPackId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassPackPurchaseResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<ClassBalanceResponse?> GetClassBalanceAsync(this HttpClient httpClient, Guid clientId) =>
        httpClient.GetFromJsonAsync<ClassBalanceResponse>(
            new Uri($"{ApiRoutes.Clients}/{clientId}{ApiRoutes.ClassBalance}", UriKind.Relative), ApiRequests.JsonOptions);
}
