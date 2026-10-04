using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record ReschedulePrivateLessonRequest(
    Guid? InstructorId,
    DateOnly Date,
    string? StartTime,
    int? DurationMinutes,
    string? Location,
    string? Notes,
    bool IsTrial = false,
    decimal? TrialPrice = null);

public sealed record ReschedulePrivateLessonCommand(Guid PrivateLessonId, ReschedulePrivateLessonRequest Details) : ICommand;

public sealed class ReschedulePrivateLessonUseCase(
    IPrivateLessonRepository privateLessonRepository,
    IInstructorRepository instructorRepository,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<ReschedulePrivateLessonCommand, PrivateLessonResponse>
{
    public async Task<Result<PrivateLessonResponse>> ExecuteAsync(ReschedulePrivateLessonCommand command, CancellationToken cancellationToken)
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

        var details = command.Details;
        var schedule = ClassSchedule.Create([details.Date.DayOfWeek], details.StartTime, details.DurationMinutes);
        if (schedule.IsFailure)
        {
            return schedule.Error!;
        }

        var instructor = await ClassGroupRules.FindActiveInstructorAsync(instructorRepository, details.InstructorId, cancellationToken);
        if (instructor.IsFailure)
        {
            return instructor.Error!;
        }

        if (!scope.Includes(instructor.Value!.Id))
        {
            return AccessRules.NotYours();
        }

        var conflict = await InstructorAgendaRules.FindConflictAsync(
            new InstructorAgendaRepositories(classGroupRepository, sessionRepository, privateLessonRepository),
            instructor.Value!.Id,
            [details.Date],
            schedule.Value!,
            lesson.Id,
            cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

        var reschedule = lesson.Reschedule(instructor.Value.Id, details.Date, schedule.Value!, details.Location, details.Notes);
        if (reschedule.IsFailure)
        {
            return reschedule.Error!;
        }

        var trial = lesson.SetTrial(details.IsTrial, details.TrialPrice);
        if (trial.IsFailure)
        {
            return trial.Error!;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var today = await businessCalendar.TodayAsync(cancellationToken);
        return await PrivateLessonRules.ToResponseAsync(lesson, instructorRepository, studentRepository, today, cancellationToken);
    }
}
