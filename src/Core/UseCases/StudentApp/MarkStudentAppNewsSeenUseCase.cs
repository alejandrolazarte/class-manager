using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record MarkStudentAppNewsSeenCommand;

public sealed class MarkStudentAppNewsSeenUseCase(
    IStudentAppAccess studentAppAccess,
    IClientAccountRepository clientAccountRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<MarkStudentAppNewsSeenCommand, bool>
{
    private const string NoAccessMessage = "This account is not linked to a client.";
    private const string NoAccessCode = "student.no_access";

    public async Task<Result<bool>> ExecuteAsync(MarkStudentAppNewsSeenCommand command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
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
