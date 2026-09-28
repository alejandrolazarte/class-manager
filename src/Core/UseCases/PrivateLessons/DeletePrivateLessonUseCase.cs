using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record DeletePrivateLessonCommand(Guid PrivateLessonId);

public sealed record DeletedPrivateLessonResponse(Guid Id);

public sealed class DeletePrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IClassPackPurchaseRepository purchaseRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes)
    : IUseCase<DeletePrivateLessonCommand, DeletedPrivateLessonResponse>
{
    private const string HasAttendanceMessage = "Attendance was already taken for this lesson.";
    private const string DeductedTrialMessage = "This trial class was deducted from a pack sale. Delete that sale first.";

    public async Task<Result<DeletedPrivateLessonResponse>> ExecuteAsync(DeletePrivateLessonCommand command, CancellationToken cancellationToken)
    {
        var lesson = await privateLessonRepository.GetForUpdateAsync(command.PrivateLessonId, cancellationToken);
        if (lesson is null)
        {
            return PrivateLessonRules.NotFound();
        }

        var scope = await accessScopes.ForInstructorsAsync(Permissions.PrivateLessons.ManageAll, cancellationToken);
        if (!scope.Includes(lesson.InstructorId))
        {
            return AccessRules.NotYours();
        }

        if (lesson.HasAttendance)
        {
            return Result.Conflict<DeletedPrivateLessonResponse>(HasAttendanceMessage, SessionErrorCodes.HasAttendance);
        }

        if (await purchaseRepository.IsTrialDeductedAsync(lesson.Id, cancellationToken))
        {
            return Result.Conflict<DeletedPrivateLessonResponse>(DeductedTrialMessage, ClassPackErrorCodes.TrialNotDeductible);
        }

        privateLessonRepository.Remove(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeletedPrivateLessonResponse(lesson.Id);
    }
}
