using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record DeletePrivateLessonCommand(Guid PrivateLessonId);

public sealed record DeletedPrivateLessonResponse(Guid Id);

public sealed class DeletePrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<DeletePrivateLessonCommand, DeletedPrivateLessonResponse>
{
    private const string HasAttendanceMessage = "Attendance was already taken for this lesson.";

    public async Task<Result<DeletedPrivateLessonResponse>> ExecuteAsync(DeletePrivateLessonCommand command, CancellationToken cancellationToken)
    {
        var lesson = await privateLessonRepository.GetForUpdateAsync(command.PrivateLessonId, cancellationToken);
        if (lesson is null)
        {
            return PrivateLessonRules.NotFound();
        }

        if (lesson.HasAttendance)
        {
            return Result.Conflict<DeletedPrivateLessonResponse>(HasAttendanceMessage, SessionErrorCodes.HasAttendance);
        }

        privateLessonRepository.Remove(lesson);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeletedPrivateLessonResponse(lesson.Id);
    }
}
