using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record BeginPasskeyRegistrationCommand([property: AuditSecret] string? CurrentPassword) : ICommand;

public sealed class BeginPasskeyRegistrationUseCase(ICurrentUser currentUser, IPasskeyIdentityService passkeyIdentityService)
    : IUseCase<BeginPasskeyRegistrationCommand, PasskeyCeremonyResponse>
{
    private const string CurrentPasswordRequiredMessage = "Write your current password.";

    public async Task<Result<PasskeyCeremonyResponse>> ExecuteAsync(BeginPasskeyRegistrationCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return Result.Unauthorized<PasskeyCeremonyResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        if (string.IsNullOrEmpty(command.CurrentPassword))
        {
            return Result.Validation<PasskeyCeremonyResponse>(
                CurrentPasswordRequiredMessage, fieldName: nameof(BeginPasskeyRegistrationCommand.CurrentPassword));
        }

        var ceremony = await passkeyIdentityService.BeginRegistrationAsync(accountId, command.CurrentPassword, cancellationToken);

        return ceremony.IsFailure ? ceremony.Error! : PasskeyCeremonyResponse.From(ceremony.Value!);
    }
}
