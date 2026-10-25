using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Passkeys;

public sealed record ListMyPasskeysQuery : IQuery;

public sealed class ListMyPasskeysUseCase(ICurrentUser currentUser, IPasskeyIdentityService passkeyIdentityService)
    : IUseCase<ListMyPasskeysQuery, IReadOnlyList<PasskeyResponse>>
{
    public async Task<Result<IReadOnlyList<PasskeyResponse>>> ExecuteAsync(ListMyPasskeysQuery query, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return Result.Unauthorized<IReadOnlyList<PasskeyResponse>>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var passkeys = await passkeyIdentityService.ListAsync(accountId, cancellationToken);

        return Result.Success<IReadOnlyList<PasskeyResponse>>([.. passkeys.Select(PasskeyResponse.From)]);
    }
}
