using System.Text.Json;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Authentication;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record SignInWithPasskeyCommand(
    [property: AuditSecret] string? CeremonyId,
    [property: AuditSecret] JsonElement? Credential)
    : ICommand;

public sealed class SignInWithPasskeyUseCase(
    IPasskeyIdentityService passkeyIdentityService,
    ISignInSessionService signInSessionService)
    : IUseCase<SignInWithPasskeyCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(SignInWithPasskeyCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.CeremonyId) || !PasskeyCommands.HasCredential(command.Credential))
        {
            return SignInSessionService.InvalidCredentials();
        }

        var verification = await passkeyIdentityService.VerifyPasskeyAsync(
            command.CeremonyId, command.Credential!.Value.GetRawText(), cancellationToken);

        return await signInSessionService.StartAsync(verification, cancellationToken);
    }
}
