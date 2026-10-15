using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.UseCases.Accounts;

public sealed record UpdateMyLanguageCommand(string? PreferredLanguage) : ICommand;

public sealed class UpdateMyLanguageUseCase(
    ICurrentUser currentUser,
    IIdentityService identityService,
    ILegalTermsAcceptanceRepository legalTermsAcceptanceRepository)
    : IUseCase<UpdateMyLanguageCommand, MyAccountResponse>
{
    private const string UnsupportedLanguageMessage = "Preferred language must be one of the supported languages.";

    public async Task<Result<MyAccountResponse>> ExecuteAsync(UpdateMyLanguageCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return NoAccess();
        }

        if (!AccountLanguage.IsSupported(command.PreferredLanguage))
        {
            return Result.Validation<MyAccountResponse>(
                UnsupportedLanguageMessage,
                fieldName: nameof(UpdateMyLanguageCommand.PreferredLanguage));
        }

        if (!await identityService.UpdatePreferredLanguageAsync(userId, command.PreferredLanguage!, cancellationToken))
        {
            return NoAccess();
        }

        var accounts = await identityService.ListAccountsAsync([userId], cancellationToken);
        return await MyAccountResponse.ForAsync(accounts[0], legalTermsAcceptanceRepository, cancellationToken);
    }

    private static Result<MyAccountResponse> NoAccess() =>
        Result.Unauthorized<MyAccountResponse>(MemberErrorCodes.NoAccessMessage, MemberErrorCodes.NoAccess);
}
