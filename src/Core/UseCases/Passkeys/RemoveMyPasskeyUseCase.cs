using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Audit;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record RemoveMyPasskeyCommand([property: AuditPlain] string PasskeyId) : ICommand;

public sealed record RemoveMyPasskeyResponse;

public sealed class RemoveMyPasskeyUseCase(ICurrentUser currentUser, IPasskeyIdentityService passkeyIdentityService)
    : IUseCase<RemoveMyPasskeyCommand, RemoveMyPasskeyResponse>
{
    public async Task<Result<RemoveMyPasskeyResponse>> ExecuteAsync(RemoveMyPasskeyCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return Result.Unauthorized<RemoveMyPasskeyResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        if (!await passkeyIdentityService.RemoveAsync(accountId, command.PasskeyId, cancellationToken))
        {
            return Result.NotFound<RemoveMyPasskeyResponse>(
                AuthenticationErrorCodes.PasskeyNotFoundMessage, AuthenticationErrorCodes.PasskeyNotFound);
        }

        return new RemoveMyPasskeyResponse();
    }
}
