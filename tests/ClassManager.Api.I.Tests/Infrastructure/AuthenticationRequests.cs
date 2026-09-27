using ClassManager.Api.Commands;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class AuthenticationRequests
{
    public const string OwnerFullName = "Laura Gómez";
    public const string OwnerPassword = "a long passphrase";
    public const string WrongPassword = "not the right passphrase";

    public const string SignUpRoute = ApiRoutes.Authentication + ApiRoutes.SignUp;
    public const string SignInRoute = ApiRoutes.Authentication + ApiRoutes.SignIn;
    public const string RefreshRoute = ApiRoutes.Authentication + ApiRoutes.Refresh;
    public const string SignOutRoute = ApiRoutes.Authentication + ApiRoutes.SignOut;
    public const string PasswordResetRoute = ApiRoutes.Authentication + ApiRoutes.PasswordReset;
    public const string WebAppResetPasswordUrl = "http://localhost:8081" + PasswordResetLinkCommand.ResetPasswordPath;

    public static string UniqueEmail() => $"owner-{Guid.NewGuid():N}@example.com";

    public static string UniqueBusinessName() => $"Panadería {Guid.NewGuid():N}";

    public static SignUpOwnerCommand SignUpCommand(string? email = null, string? businessName = null) =>
        new(
            OwnerFullName,
            email ?? UniqueEmail(),
            OwnerPassword,
            businessName ?? UniqueBusinessName(),
            ApiFixture.BuenosAiresTimeZoneId,
            ApiFixture.CurrencyCode,
            ApiFixture.DefaultCountryCallingCode);

    public static Task<HttpResponseMessage> PostSignUpAsync(this HttpClient httpClient, SignUpOwnerCommand command) =>
        httpClient.PostAsJsonAsync(SignUpRoute, command, ApiRequests.JsonOptions);

    public static async Task<TokenResponse> SignUpAsync(this HttpClient httpClient, SignUpOwnerCommand command)
    {
        using var response = await httpClient.PostSignUpAsync(command);
        response.EnsureSuccessStatusCode();
        return await response.ReadTokensAsync();
    }

    public static Task<HttpResponseMessage> PostSignInAsync(this HttpClient httpClient, string email, string password) =>
        httpClient.PostAsJsonAsync(SignInRoute, new SignInCommand(email, password), ApiRequests.JsonOptions);

    public static async Task<TokenResponse> SignInAsync(this HttpClient httpClient, string email, string password = OwnerPassword)
    {
        using var response = await httpClient.PostSignInAsync(email, password);
        response.EnsureSuccessStatusCode();
        return await response.ReadTokensAsync();
    }

    public static Task<HttpResponseMessage> PostRefreshAsync(this HttpClient httpClient, string refreshToken) =>
        httpClient.PostAsJsonAsync(RefreshRoute, new RefreshSessionCommand(refreshToken), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostSignOutAsync(this HttpClient httpClient, string refreshToken) =>
        httpClient.PostAsJsonAsync(SignOutRoute, new SignOutCommand(refreshToken), ApiRequests.JsonOptions);

    public static async Task<TokenResponse> ReadTokensAsync(this HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PostPasswordResetAsync(this HttpClient httpClient, string token, string newPassword) =>
        httpClient.PostAsJsonAsync(PasswordResetRoute, new ResetPasswordCommand(token, newPassword), ApiRequests.JsonOptions);

    public static async Task<string> CreatePasswordResetLinkAsync(this IServiceProvider services, string email)
    {
        await using var output = new StringWriter();
        var exitCode = await PasswordResetLinkCommand.RunAsync(services, [PasswordResetLinkCommand.Name, email], output);
        exitCode.ShouldBe(0);
        return output.ToString().Trim();
    }

    public static string TokenOf(string passwordResetLink) =>
        passwordResetLink[(passwordResetLink.IndexOf('=', StringComparison.Ordinal) + 1)..];
}
