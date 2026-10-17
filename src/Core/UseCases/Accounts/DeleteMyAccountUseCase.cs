using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record DeleteMyAccountCommand(string? CurrentPassword) : ICommand;

public sealed record DeleteMyAccountResponse;

public sealed class DeleteMyAccountUseCase(
    ICurrentUser currentUser,
    IIdentityService identityService,
    IOrganizationMemberRepository organizationMemberRepository,
    IAccountLinkRepository accountLinkRepository,
    TimeProvider timeProvider)
    : IUseCase<DeleteMyAccountCommand, DeleteMyAccountResponse>
{
    private const string CurrentPasswordRequiredMessage = "Write your current password.";
    private const string AccountNotDeletedMessage = "The account could not be deleted.";

    public async Task<Result<DeleteMyAccountResponse>> ExecuteAsync(DeleteMyAccountCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.AccountId is not { } accountId)
        {
            return NoAccess();
        }

        if (string.IsNullOrEmpty(command.CurrentPassword))
        {
            return Result.Validation<DeleteMyAccountResponse>(
                CurrentPasswordRequiredMessage, fieldName: nameof(DeleteMyAccountCommand.CurrentPassword));
        }

        if (!await identityService.IsPasswordCorrectAsync(accountId, command.CurrentPassword, cancellationToken))
        {
            return Result.Validation<DeleteMyAccountResponse>(
                AuthenticationErrorCodes.InvalidCurrentPasswordMessage,
                AuthenticationErrorCodes.InvalidCurrentPassword,
                nameof(DeleteMyAccountCommand.CurrentPassword));
        }

        if (await organizationMemberRepository.IsTheOnlyBrandOwnerOfABrandAsync(accountId, cancellationToken))
        {
            return Result.Conflict<DeleteMyAccountResponse>(BranchErrorCodes.LastBrandOwnerMessage, BranchErrorCodes.LastBrandOwner);
        }

        await accountLinkRepository.UnlinkFromEveryBusinessAsync(accountId, timeProvider.GetUtcNow(), cancellationToken);
        if (!await identityService.DeleteAccountAsync(accountId, cancellationToken))
        {
            throw new InvalidOperationException(AccountNotDeletedMessage);
        }

        return new DeleteMyAccountResponse();
    }

    private static Result<DeleteMyAccountResponse> NoAccess() =>
        Result.Unauthorized<DeleteMyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
}
