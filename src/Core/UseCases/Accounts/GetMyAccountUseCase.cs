using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record GetMyAccountQuery : IQuery;

public sealed record MyAccountResponse(
    string Email,
    string FullName,
    DateOnly? BirthDate,
    bool HasAcceptedCurrentLegalTerms,
    string PreferredLanguage)
{
    public static async Task<MyAccountResponse> ForAsync(
        UserAccount account,
        ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository,
        CancellationToken cancellationToken) =>
        new(
            account.Email,
            account.FullName,
            account.BirthDate,
            await legalTermsAcceptanceRepository.HasAcceptedAsync(account.UserId, LegalTerms.CurrentVersion, cancellationToken),
            account.PreferredLanguage);
}

public sealed class GetMyAccountUseCase(
    ICurrentUser currentUser,
    IIdentityService identityService,
    ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository)
    : IUseCase<GetMyAccountQuery, MyAccountResponse>
{
    public async Task<Result<MyAccountResponse>> ExecuteAsync(GetMyAccountQuery command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var accounts = await identityService.ListAccountsAsync([userId], cancellationToken);
        if (accounts.Count == 0)
        {
            return Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        return await MyAccountResponse.ForAsync(accounts[0], legalTermsAcceptanceRepository, cancellationToken);
    }
}
