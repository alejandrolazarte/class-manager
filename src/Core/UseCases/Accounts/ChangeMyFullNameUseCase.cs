using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record ChangeMyFullNameCommand(string? FullName) : ICommand;

public sealed class ChangeMyFullNameUseCase(ICurrentUser currentUser, IIdentityService identityService)
    : IUseCase<ChangeMyFullNameCommand, MyAccountResponse>
{
    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";

    public async Task<Result<MyAccountResponse>> ExecuteAsync(ChangeMyFullNameCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return NoAccess();
        }

        var fullName = command.FullName?.Trim() ?? string.Empty;
        if (fullName.Length is < OwnerAccount.FullNameMinLength or > OwnerAccount.FullNameMaxLength)
        {
            return Result.Validation<MyAccountResponse>(FullNameLengthMessage, fieldName: nameof(ChangeMyFullNameCommand.FullName));
        }

        if (!await identityService.ChangeFullNameAsync(userId, fullName, cancellationToken))
        {
            return NoAccess();
        }

        var accounts = await identityService.ListAccountsAsync([userId], cancellationToken);
        return new MyAccountResponse(accounts[0].Email, accounts[0].FullName);
    }

    private static Result<MyAccountResponse> NoAccess() =>
        Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
}
