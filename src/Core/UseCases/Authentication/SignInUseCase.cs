using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record SignInCommand([property: AuditPersonal] string? Email, [property: AuditSecret] string? Password) : ICommand;

public sealed class SignInUseCase(
    IIdentityService identityService,
    ISignInSessionService signInSessionService)
    : IUseCase<SignInCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrEmpty(command.Password))
        {
            return SignInSessionService.InvalidCredentials();
        }

        var verification = await identityService.VerifyCredentialsAsync(command.Email.Trim(), command.Password, cancellationToken);

        return await signInSessionService.StartAsync(verification, cancellationToken);
    }
}
