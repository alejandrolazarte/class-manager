using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record GetMyAccountQuery : IQuery;

public sealed record MyAccountResponse(string Email, string FullName, DateOnly? BirthDate)
{
    public static MyAccountResponse From(UserAccount account) => new(account.Email, account.FullName, account.BirthDate);
}

public sealed class GetMyAccountUseCase(ICurrentUser currentUser, IIdentityService identityService)
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

        return MyAccountResponse.From(accounts[0]);
    }
}
