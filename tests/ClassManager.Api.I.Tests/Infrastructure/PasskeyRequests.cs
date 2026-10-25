using System.Text.Json;
using System.Text.Json.Nodes;
using ClassManager.Core.UseCases.Passkeys;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class PasskeyRequests
{
    public const string PasskeyName = "iPhone";
    public const string RegistrationOptionsRoute = ApiRoutes.MyPasskeys + ApiRoutes.PasskeyRegistrationOptions;
    public const string SignInOptionsRoute = ApiRoutes.Authentication + ApiRoutes.PasskeySignInOptions;
    public const string SignInRoute = ApiRoutes.Authentication + ApiRoutes.PasskeySignIn;

    public static Task<HttpResponseMessage> PostRegistrationOptionsAsync(this HttpClient httpClient, string currentPassword) =>
        httpClient.PostAsJsonAsync(RegistrationOptionsRoute, new BeginPasskeyRegistrationCommand(currentPassword), ApiRequests.JsonOptions);

    public static async Task<PasskeyCeremonyResponse> BeginRegistrationAsync(
        this HttpClient httpClient,
        string currentPassword = AuthenticationRequests.OwnerPassword)
    {
        using var response = await httpClient.PostRegistrationOptionsAsync(currentPassword);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PasskeyCeremonyResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostPasskeyAsync(
        this HttpClient httpClient,
        string ceremonyId,
        JsonObject credential,
        string name = PasskeyName) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.MyPasskeys,
            new FinishPasskeyRegistrationCommand(ceremonyId, ToElement(credential), name),
            ApiRequests.JsonOptions);

    public static async Task<PasskeyResponse> AddPasskeyAsync(
        this HttpClient httpClient,
        SoftwareAuthenticator authenticator,
        string currentPassword = AuthenticationRequests.OwnerPassword)
    {
        var ceremony = await httpClient.BeginRegistrationAsync(currentPassword);
        using var response = await httpClient.PostPasskeyAsync(ceremony.CeremonyId, authenticator.CreateCredential(ceremony.Options));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PasskeyResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<IReadOnlyList<PasskeyResponse>> ListPasskeysAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<IReadOnlyList<PasskeyResponse>>(ApiRoutes.MyPasskeys, ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> DeletePasskeyAsync(this HttpClient httpClient, string passkeyId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.MyPasskeys}/{passkeyId}", UriKind.Relative));

    public static async Task<PasskeyCeremonyResponse> BeginPasskeySignInAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.PostAsync(new Uri(SignInOptionsRoute, UriKind.Relative), null);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PasskeyCeremonyResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostPasskeySignInAsync(this HttpClient httpClient, string ceremonyId, JsonObject credential) =>
        httpClient.PostAsJsonAsync(SignInRoute, SignInCommand(ceremonyId, credential), ApiRequests.JsonOptions);

    public static async Task<HttpResponseMessage> SignInWithPasskeyAsync(this HttpClient httpClient, SoftwareAuthenticator authenticator)
    {
        var ceremony = await httpClient.BeginPasskeySignInAsync();
        return await httpClient.PostPasskeySignInAsync(ceremony.CeremonyId, authenticator.Sign(ceremony.Options));
    }

    public static SignInWithPasskeyCommand SignInCommand(string ceremonyId, JsonObject credential) => new(ceremonyId, ToElement(credential));

    private static JsonElement ToElement(JsonObject credential) => JsonSerializer.SerializeToElement(credential);
}
