using System.Buffers.Text;
using System.Security.Cryptography;
using ClassManager.Core.UseCases.Families;
using ClassManager.Infrastructure.WebPush;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class PushRequests
{
    private const int AuthSecretLength = 16;

    private static string SubscriptionPath => ApiRoutes.Family + ApiRoutes.PushSubscription;

    public static SavePushSubscriptionCommand NewSubscription()
    {
        using var userAgentKey = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        return new SavePushSubscriptionCommand(
            $"https://push.example.com/send/{Guid.NewGuid():N}",
            Base64Url.EncodeToString(EcKeys.ToUncompressedPoint(userAgentKey.ExportParameters(false).Q)),
            Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(AuthSecretLength)));
    }

    public static Task<HttpResponseMessage> PutPushSubscriptionAsync(this HttpClient httpClient, SavePushSubscriptionCommand subscription) =>
        httpClient.PutAsJsonAsync(SubscriptionPath, subscription, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeletePushSubscriptionAsync(this HttpClient httpClient, string endpoint) =>
        httpClient.DeleteAsync(new Uri($"{SubscriptionPath}?endpoint={Uri.EscapeDataString(endpoint)}", UriKind.Relative));

    public static Task<FamilyPushKeyResponse?> GetPushKeyAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<FamilyPushKeyResponse>(new Uri(ApiRoutes.Family + ApiRoutes.PushKey, UriKind.Relative), ApiRequests.JsonOptions);

    public static async Task<string> SubscribeAsync(this FamilyScenario scenario)
    {
        var subscription = NewSubscription();
        (await scenario.Family.PutPushSubscriptionAsync(subscription)).EnsureSuccessStatusCode();
        return subscription.Endpoint!;
    }
}
