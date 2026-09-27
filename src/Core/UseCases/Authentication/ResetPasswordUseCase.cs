using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Authentication;

public sealed record ResetPasswordCommand(string? Token, string? NewPassword);

public sealed record ResetPasswordResponse;

public sealed class ResetPasswordUseCase(IIdentityService identityService) : IUseCase<ResetPasswordCommand, ResetPasswordResponse>
{
    public const string NewPasswordRequiredMessage = "Enter the new password.";

    public async Task<Result<ResetPasswordResponse>> ExecuteAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Token))
        {
            return Result.Validation<ResetPasswordResponse>(
                AuthenticationErrorCodes.InvalidPasswordResetTokenMessage,
                AuthenticationErrorCodes.InvalidPasswordResetToken);
        }

        if (string.IsNullOrEmpty(command.NewPassword))
        {
            return Result.Validation<ResetPasswordResponse>(
                NewPasswordRequiredMessage,
                fieldName: nameof(ResetPasswordCommand.NewPassword));
        }

        var reset = await identityService.ResetPasswordAsync(command.Token.Trim(), command.NewPassword, cancellationToken);

        return reset.IsSuccess ? new ResetPasswordResponse() : reset.Error!;
    }
}
