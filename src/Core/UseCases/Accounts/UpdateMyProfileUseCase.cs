using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record UpdateMyProfileCommand(string? FullName, DateOnly? BirthDate) : ICommand;

public sealed class UpdateMyProfileUseCase(ICurrentUser currentUser, IIdentityService identityService, TimeProvider timeProvider)
    : IUseCase<UpdateMyProfileCommand, MyAccountResponse>
{
    private const string FullNameLengthMessage = "Full name must be between 2 and 120 characters.";
    private const string BirthDateMessage = "Birth date is required and must be between 1900-01-01 and today.";

    public async Task<Result<MyAccountResponse>> ExecuteAsync(UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return NoAccess();
        }

        var fullName = command.FullName?.Trim() ?? string.Empty;
        if (fullName.Length is < OwnerAccount.FullNameMinLength or > OwnerAccount.FullNameMaxLength)
        {
            return Result.Validation<MyAccountResponse>(FullNameLengthMessage, fieldName: nameof(UpdateMyProfileCommand.FullName));
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (command.BirthDate is not { } birthDate || !PersonAge.IsValidBirthDate(birthDate, today))
        {
            return Result.Validation<MyAccountResponse>(BirthDateMessage, fieldName: nameof(UpdateMyProfileCommand.BirthDate));
        }

        if (!await identityService.UpdateProfileAsync(userId, fullName, birthDate, cancellationToken))
        {
            return NoAccess();
        }

        var accounts = await identityService.ListAccountsAsync([userId], cancellationToken);
        return MyAccountResponse.From(accounts[0]);
    }

    private static Result<MyAccountResponse> NoAccess() =>
        Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
}
