using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record AcceptLegalTermsCommand : ICommand;

public sealed record AcceptLegalTermsResponse(string Version);

public sealed class AcceptLegalTermsUseCase(
    ICurrentUser currentUser,
    ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<AcceptLegalTermsCommand, AcceptLegalTermsResponse>
{
    public async Task<Result<AcceptLegalTermsResponse>> ExecuteAsync(AcceptLegalTermsCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } userId)
        {
            return Result.Unauthorized<AcceptLegalTermsResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        if (!await legalTermsAcceptanceRepository.HasAcceptedAsync(userId, LegalTerms.CurrentVersion, cancellationToken))
        {
            legalTermsAcceptanceRepository.Add(LegalTermsAcceptance.RecordCurrent(userId, timeProvider.GetUtcNow()));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new AcceptLegalTermsResponse(LegalTerms.CurrentVersion);
    }
}
