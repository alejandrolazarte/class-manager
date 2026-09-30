using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Families;

public sealed record MarkFamilyNewsSeenCommand;

public sealed class MarkFamilyNewsSeenUseCase(
    IFamilyAccess familyAccess,
    IClientAccountRepository clientAccountRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<MarkFamilyNewsSeenCommand, bool>
{
    private const string NoAccessMessage = "This account is not linked to a family.";
    private const string NoAccessCode = "family.no_access";

    public async Task<Result<bool>> ExecuteAsync(MarkFamilyNewsSeenCommand command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        var account = access is null ? null : await clientAccountRepository.FindByUserForUpdateAsync(access.UserId, cancellationToken);
        if (account is null)
        {
            return Result.Unauthorized<bool>(NoAccessMessage, NoAccessCode);
        }

        account.MarkNewsSeen(timeProvider.GetUtcNow());
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
