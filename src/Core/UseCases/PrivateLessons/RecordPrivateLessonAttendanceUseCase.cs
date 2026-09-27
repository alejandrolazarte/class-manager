using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record RecordPrivateLessonAttendanceCommand(Guid PrivateLessonId, Guid StudentId, AttendanceStatus? Status);

public sealed class RecordPrivateLessonAttendanceUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<RecordPrivateLessonAttendanceCommand, RecordAttendanceResponse>
{
    private const string InFutureMessage = "Attendance can't be taken before the lesson date.";
    private const string DateFieldName = "Date";

    public async Task<Result<RecordAttendanceResponse>> ExecuteAsync(RecordPrivateLessonAttendanceCommand command, CancellationToken cancellationToken)
    {
        var lesson = await privateLessonRepository.GetForUpdateAsync(command.PrivateLessonId, cancellationToken);
        if (lesson is null)
        {
            return PrivateLessonRules.NotFound();
        }

        if (lesson.Date > await businessCalendar.TodayAsync(cancellationToken))
        {
            return Result.Validation<RecordAttendanceResponse>(InFutureMessage, SessionErrorCodes.InFuture, DateFieldName);
        }

        var mark = lesson.Mark(command.StudentId, command.Status);
        if (mark.IsFailure)
        {
            return mark.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RecordAttendanceResponse(command.StudentId, command.Status);
    }
}
