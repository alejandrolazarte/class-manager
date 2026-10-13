using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record AcceptLegalTermsCommand : ICommand;

public sealed record AcceptLegalTermsResponse(string Version);

public sealed class AcceptLegalTermsUseCase(ICurrentUser currentUser, IIdentityService identityService, TimeProvider timeProvider)
    : IUseCase<AcceptLegalTermsCommand, AcceptLegalTermsResponse>
{
    public async Task<Result<AcceptLegalTermsResponse>> ExecuteAsync(AcceptLegalTermsCommand command, CancellationToken cancellationToken)
    {
        var acceptance = LegalTermsAcceptance.CurrentAt(timeProvider.GetUtcNow());
        if (currentUser.UserId is not { } userId
            || !await identityService.RecordLegalTermsAcceptanceAsync(userId, acceptance, cancellationToken))
        {
            return Result.Unauthorized<AcceptLegalTermsResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        return new AcceptLegalTermsResponse(acceptance.Version);
    }
}
