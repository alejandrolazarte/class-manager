using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record CancelPrivateLessonRequest(string? Reason);

public sealed record CancelPrivateLessonCommand(Guid PrivateLessonId, string? Reason) : ICommand;

public sealed class CancelPrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IStudentRepository studentRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<CancelPrivateLessonCommand, PrivateLessonResponse>
{
    public async Task<Result<PrivateLessonResponse>> ExecuteAsync(CancelPrivateLessonCommand command, CancellationToken cancellationToken)
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
