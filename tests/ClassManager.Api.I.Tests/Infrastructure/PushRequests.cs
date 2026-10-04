using System.Buffers.Text;
using System.Security.Cryptography;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class PushRequests
{
    private const int AuthSecretLength = 16;

    private static string SubscriptionPath => ApiRoutes.StudentApp + ApiRoutes.PushSubscription;

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

    public static Task<StudentAppPushKeyResponse?> GetPushKeyAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<StudentAppPushKeyResponse>(new Uri(ApiRoutes.StudentApp + ApiRoutes.PushKey, UriKind.Relative), ApiRequests.JsonOptions);

    public static async Task<string> SubscribeAsync(this StudentAppScenario scenario)
    {
        var subscription = NewSubscription();
        (await scenario.Student.PutPushSubscriptionAsync(subscription)).EnsureSuccessStatusCode();
        return subscription.Endpoint!;
    }
}
