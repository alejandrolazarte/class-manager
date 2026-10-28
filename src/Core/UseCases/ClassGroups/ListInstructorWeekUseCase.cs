using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Instructors;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.UseCases.ClassGroups;

public sealed record ListInstructorWeekQuery(long? InstructorId, long? IgnoredClassGroupId = null) : IQuery;

public sealed record InstructorWeekSlotResponse(
    DayOfWeek Weekday,
    string StartTime,
    int DurationMinutes,
    InstructorAgendaSlotKind Kind,
    string? ClassGroupName);

public sealed class ListInstructorWeekUseCase(
    IInstructorRepository instructorRepository,
    IClassGroupRepository classGroupRepository,
    IPrivateLessonRepository privateLessonRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<ListInstructorWeekQuery, IReadOnlyList<InstructorWeekSlotResponse>>
{
    public async Task<Result<IReadOnlyList<InstructorWeekSlotResponse>>> ExecuteAsync(
        ListInstructorWeekQuery command,
        CancellationToken cancellationToken)
    {
        var instructor = command.InstructorId is { } instructorId
            ? await instructorRepository.GetByIdAsync(instructorId, cancellationToken)
            : null;
        if (instructor is null)
        {
            return InstructorFailures.NotFound();
        }

        var classGroups = await classGroupRepository.ListActiveByInstructorAsync(instructor.Id, cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var upcomingLessons = await privateLessonRepository.ListByInstructorFromAsync(instructor.Id, today, cancellationToken);

        var classSlots = classGroups
            .Where(classGroup => classGroup.Id != command.IgnoredClassGroupId)
            .SelectMany(classGroup => classGroup.Schedule.Days.Select(weekday => new InstructorWeekSlotResponse(
                weekday,
                PrivateLessonResponse.FormatTime(classGroup.Schedule.StartTime),
                classGroup.Schedule.DurationMinutes,
                InstructorAgendaSlotKind.ClassGroup,
                classGroup.Name)));
        var lessonSlots = upcomingLessons
            .Select(lesson => new InstructorWeekSlotResponse(
                lesson.Date.DayOfWeek,
                PrivateLessonResponse.FormatTime(lesson.StartTime),
                lesson.DurationMinutes,
                InstructorAgendaSlotKind.PrivateLesson,
                null))
            .Distinct();

        return Result.Success<IReadOnlyList<InstructorWeekSlotResponse>>(
        [
            .. classSlots.Concat(lessonSlots)
                .OrderBy(slot => slot.Weekday)
                .ThenBy(slot => slot.StartTime, StringComparer.Ordinal),
        ]);
    }
}
