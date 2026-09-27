using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record CreatePasswordResetTokenCommand(string? Email);

public sealed record PasswordResetTokenResponse(string Token);

public sealed class CreatePasswordResetTokenUseCase(IIdentityService identityService)
    : IUseCase<CreatePasswordResetTokenCommand, PasswordResetTokenResponse>
{
    public async Task<Result<PasswordResetTokenResponse>> ExecuteAsync(
        CreatePasswordResetTokenCommand command,
        CancellationToken cancellationToken)
    {
        var token = string.IsNullOrWhiteSpace(command.Email)
            ? null
            : await identityService.CreatePasswordResetTokenAsync(command.Email.Trim(), cancellationToken);

        return token is null
            ? Result.NotFound<PasswordResetTokenResponse>(
                AuthenticationErrorCodes.AccountNotFoundMessage,
                AuthenticationErrorCodes.AccountNotFound)
            : new PasswordResetTokenResponse(token);
    }
}
