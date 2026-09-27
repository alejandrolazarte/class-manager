using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record CancelPrivateLessonRequest(string? Reason);

public sealed record CancelPrivateLessonCommand(Guid PrivateLessonId, string? Reason);

public sealed class CancelPrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IStudentRepository studentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar)
    : IUseCase<CancelPrivateLessonCommand, PrivateLessonResponse>
{
    public async Task<Result<PrivateLessonResponse>> ExecuteAsync(CancelPrivateLessonCommand command, CancellationToken cancellationToken)
    {
        var lesson = await privateLessonRepository.GetForUpdateAsync(command.PrivateLessonId, cancellationToken);
        if (lesson is null)
        {
            return PrivateLessonRules.NotFound();
        }

        var cancel = lesson.Cancel(command.Reason);
        if (cancel.IsFailure)
        {
            return cancel.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var today = await businessCalendar.TodayAsync(cancellationToken);
        return await PrivateLessonRules.ToResponseAsync(lesson, instructorRepository, studentRepository, today, cancellationToken);
    }
}
