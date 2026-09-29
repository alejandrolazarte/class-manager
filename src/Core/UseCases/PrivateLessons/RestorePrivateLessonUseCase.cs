using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record RestorePrivateLessonCommand(Guid PrivateLessonId);

public sealed class RestorePrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<RestorePrivateLessonCommand, PrivateLessonResponse>
{
    public async Task<Result<PrivateLessonResponse>> ExecuteAsync(RestorePrivateLessonCommand command, CancellationToken cancellationToken)
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

        var conflict = await InstructorAgendaRules.FindConflictAsync(
            new InstructorAgendaRepositories(classGroupRepository, sessionRepository, privateLessonRepository),
            lesson.InstructorId,
            [lesson.Date],
            ClassSchedule.ForDay(lesson.Date.DayOfWeek, lesson.StartTime, lesson.DurationMinutes),
            lesson.Id,
            cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

        lesson.Restore();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var today = await businessCalendar.TodayAsync(cancellationToken);
        return await PrivateLessonRules.ToResponseAsync(lesson, instructorRepository, studentRepository, today, cancellationToken);
    }
}
