using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Api.Commands;

public static class PasswordResetLinkCommand
{
    public const string Name = "create-password-reset-link";
    public const string WebAppUrlSetting = "WebApp:Url";
    public const string ResetPasswordPath = "/reset-password?token=";

    private const string UsageMessage = "Usage: dotnet Api.dll " + Name + " <email>";
    private const string MissingWebAppUrlMessage = "Set " + WebAppUrlSetting + " (environment variable WebApp__Url) to the web app address.";

    public static bool Matches(string[] arguments) => arguments is [Name, ..];

    public static async Task<int> RunAsync(IServiceProvider services, string[] arguments, TextWriter output)
    {
        if (arguments is not [Name, var email])
        {
            await output.WriteLineAsync(UsageMessage);
            return 1;
        }

        var webAppUrl = services.GetRequiredService<IConfiguration>()[WebAppUrlSetting];
        if (string.IsNullOrWhiteSpace(webAppUrl))
        {
            await output.WriteLineAsync(MissingWebAppUrlMessage);
            return 1;
        }

        await using var scope = services.CreateAsyncScope();
        var useCase = scope.ServiceProvider.GetRequiredService<IUseCase<CreatePasswordResetTokenCommand, PasswordResetTokenResponse>>();
        var result = await useCase.ExecuteAsync(new CreatePasswordResetTokenCommand(email), CancellationToken.None);
        if (result.IsFailure)
        {
            await output.WriteLineAsync(result.Error!.Message);
            return 1;
        }

        await output.WriteLineAsync(webAppUrl.TrimEnd('/') + ResetPasswordPath + result.Value!.Token);
        return 0;
    }
}
