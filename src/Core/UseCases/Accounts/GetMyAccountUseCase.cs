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
    public static async Task<Result<MyAccountResponse>> OfAsync(
        Guid accountId,
        IIdentityService identityService,
        IUserRepository userRepository,
        ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository,
        CancellationToken cancellationToken)
    {
        var accounts = await identityService.ListAccountsAsync([accountId], cancellationToken);
        var user = await userRepository.FindByAccountAsync(accountId, cancellationToken);
        if (accounts.Count == 0 || user is null)
        {
            return Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        var account = accounts[0];
        return new MyAccountResponse(
            account.Email,
            user.FullName,
            user.BirthDate,
            await legalTermsAcceptanceRepository.HasAcceptedAsync(account.UserId, LegalTerms.CurrentVersion, cancellationToken),
            account.PreferredLanguage);
    }
}

public sealed class GetMyAccountUseCase(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IUserRepository userRepository,
    ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository)
    : IUseCase<GetMyAccountQuery, MyAccountResponse>
{
    public async Task<Result<MyAccountResponse>> ExecuteAsync(GetMyAccountQuery command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } userId)
        {
            return Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
        }

        return await MyAccountResponse.OfAsync(userId, identityService, userRepository, legalTermsAcceptanceRepository, cancellationToken);
    }
}
