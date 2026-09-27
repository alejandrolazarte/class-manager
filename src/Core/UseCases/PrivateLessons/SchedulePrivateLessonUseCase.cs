using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;
using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Core.UseCases.PrivateLessons;

public sealed record SchedulePrivateLessonCommand(
    Guid? InstructorId,
    IReadOnlyList<Guid>? StudentIds,
    DateOnly Date,
    string? StartTime,
    int? DurationMinutes,
    string? Location,
    string? Notes,
    int? RepeatWeeks,
    bool IsTrial = false,
    decimal? TrialPrice = null);

public sealed class SchedulePrivateLessonUseCase(
    IInstructorRepository instructorRepository,
    IStudentRepository studentRepository,
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<SchedulePrivateLessonCommand, IReadOnlyList<PrivateLessonResponse>>
{
    public const int MinimumRepeatWeeks = 1;
    public const int MaximumRepeatWeeks = 52;

    private const int DaysPerWeek = 7;
    private const string RepeatWeeksMessage = "Repeat between 1 and 52 weeks.";
    private const string StudentNotFoundMessage = "A student does not exist.";

    public async Task<Result<IReadOnlyList<PrivateLessonResponse>>> ExecuteAsync(
        SchedulePrivateLessonCommand command,
        CancellationToken cancellationToken)
    {
        var repeatWeeks = command.RepeatWeeks ?? MinimumRepeatWeeks;
        if (repeatWeeks is < MinimumRepeatWeeks or > MaximumRepeatWeeks)
        {
            return Result.Validation<IReadOnlyList<PrivateLessonResponse>>(
                RepeatWeeksMessage, fieldName: nameof(SchedulePrivateLessonCommand.RepeatWeeks));
        }

        var schedule = ClassSchedule.Create([command.Date.DayOfWeek], command.StartTime, command.DurationMinutes);
        if (schedule.IsFailure)
        {
            return schedule.Error!;
        }

        var instructor = await ClassGroupRules.FindActiveInstructorAsync(instructorRepository, command.InstructorId, cancellationToken);
        if (instructor.IsFailure)
        {
            return instructor.Error!;
        }

        IReadOnlyList<Guid> studentIds = command.StudentIds ?? [];
        var students = await studentRepository.ListSummariesByIdsAsync([.. studentIds.Distinct()], cancellationToken);
        if (students.Count != studentIds.Distinct().Count())
        {
            return Result.NotFound<IReadOnlyList<PrivateLessonResponse>>(StudentNotFoundMessage, StudentErrorCodes.NotFound);
        }

        IReadOnlyList<DateOnly> dates = [.. Enumerable.Range(0, repeatWeeks).Select(week => command.Date.AddDays(week * DaysPerWeek))];
        var seriesId = repeatWeeks > MinimumRepeatWeeks ? Guid.CreateVersion7() : (Guid?)null;
        var createdAt = timeProvider.GetUtcNow();
        var lessons = new List<PrivateLesson>();
        foreach (var date in dates)
        {
            var lesson = PrivateLesson.Create(
                instructor.Value!.Id, date, schedule.Value!, studentIds, command.Location, command.Notes, seriesId, createdAt);
            if (lesson.IsFailure)
            {
                return lesson.Error!;
            }

            var trial = lesson.Value!.SetTrial(command.IsTrial, command.TrialPrice);
            if (trial.IsFailure)
            {
                return trial.Error!;
            }

            lessons.Add(lesson.Value);
        }

        var conflict = await InstructorAgendaRules.FindConflictAsync(
            new InstructorAgendaRepositories(classGroupRepository, sessionRepository, privateLessonRepository),
            instructor.Value!.Id,
            dates,
            schedule.Value!,
            ignoredPrivateLessonId: null,
            cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

        lessons.ForEach(privateLessonRepository.Add);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var today = await businessCalendar.TodayAsync(cancellationToken);
        return Result.Success<IReadOnlyList<PrivateLessonResponse>>(
            [.. lessons.Select(lesson => PrivateLessonResponse.From(lesson, instructor.Value.FullName, students, today))]);
    }
}
