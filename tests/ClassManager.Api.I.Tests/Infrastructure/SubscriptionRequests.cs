using ClassManager.Core.UseCases.Subscriptions;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SubscriptionRequests
{
    public static async Task<List<PlanResponse>> ListPlansAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<PlanResponse>>(new Uri(ApiRoutes.Plans, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> GetOrganizationSubscriptionAsync(this HttpClient httpClient) =>
        httpClient.GetAsync(new Uri(ApiRoutes.OrganizationSubscription, UriKind.Relative));
}
