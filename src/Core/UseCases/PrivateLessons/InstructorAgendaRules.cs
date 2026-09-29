using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.UseCases.PrivateLessons;

internal static class InstructorAgendaRules
{
    private const string InstructorBusyMessage = "The instructor already teaches at that time.";

    public static async Task<ResultError?> FindConflictAsync(
        InstructorAgendaRepositories repositories,
        Guid instructorId,
        IReadOnlyList<DateOnly> dates,
        ClassSchedule schedule,
        Guid? ignoredPrivateLessonId,
        CancellationToken cancellationToken)
    {
        var privateLessonConflict = await FindPrivateLessonConflictAsync(
            repositories.PrivateLessons, instructorId, dates, schedule, ignoredPrivateLessonId, cancellationToken);
        if (privateLessonConflict is not null)
        {
            return privateLessonConflict;
        }

        var overlap = await FindClassGroupOverlapAsync(
            repositories, instructorId, dates, schedule.StartTime, schedule.DurationMinutes, ignoredClassGroupId: null, cancellationToken);
        return overlap is null ? null : Busy(overlap.Date, ClassGroupErrorCodes.ConflictingClassGroupIdDetail, overlap.ClassGroup.Id);
    }

    public static async Task<ClassGroupOverlap?> FindClassGroupOverlapAsync(
        InstructorAgendaRepositories repositories,
        Guid instructorId,
        IReadOnlyList<DateOnly> dates,
        TimeOnly startTime,
        int durationMinutes,
        Guid? ignoredClassGroupId,
        CancellationToken cancellationToken)
    {
        var firstDate = dates.Min();
        var lastDate = dates.Max();
        var usualClassGroups = await repositories.ClassGroups.ListActiveByInstructorAsync(instructorId, cancellationToken);
        var substitutions = await repositories.Sessions.ListSubstitutionsAsync(instructorId, firstDate, lastDate, cancellationToken);
        if (usualClassGroups.Count == 0 && substitutions.Count == 0)
        {
            return null;
        }

        var classGroupsById = usualClassGroups.ToDictionary(classGroup => classGroup.Id);
        foreach (var classGroupId in substitutions.Select(session => session.ClassGroupId).Distinct())
        {
            if (!classGroupsById.ContainsKey(classGroupId)
                && await repositories.ClassGroups.GetByIdAsync(classGroupId, cancellationToken) is { IsActive: true } substitutedClassGroup)
            {
                classGroupsById[classGroupId] = substitutedClassGroup;
            }
        }

        var sessions = (await repositories.Sessions.ListBetweenAsync(firstDate, lastDate, cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        foreach (var date in dates)
        {
            var requestedTime = ClassSchedule.ForDay(date.DayOfWeek, startTime, durationMinutes);
            var overlappingClassGroup = classGroupsById.Values
                .Where(classGroup => classGroup.Id != ignoredClassGroupId && classGroup.Schedule.MeetsOn(date.DayOfWeek))
                .FirstOrDefault(classGroup =>
                {
                    var session = sessions.GetValueOrDefault((classGroup.Id, date));
                    if (session?.IsCancelled == true
                        || (session?.EffectiveInstructorId(classGroup.InstructorId) ?? classGroup.InstructorId) != instructorId)
                    {
                        return false;
                    }

                    var sessionStartTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
                    return ClassSchedule.ForDay(date.DayOfWeek, sessionStartTime, classGroup.DurationMinutes).OverlapsWith(requestedTime);
                });
            if (overlappingClassGroup is not null)
            {
                return new ClassGroupOverlap(date, overlappingClassGroup);
            }
        }

        return null;
    }

    public static async Task<ResultError?> FindPrivateLessonConflictAsync(
        IPrivateLessonRepository privateLessonRepository,
        Guid instructorId,
        IReadOnlyList<DateOnly> dates,
        ClassSchedule schedule,
        Guid? ignoredPrivateLessonId,
        CancellationToken cancellationToken)
    {
        var privateLessons = await privateLessonRepository.ListByInstructorOnDatesAsync(instructorId, dates, cancellationToken);
        var overlappingLesson = privateLessons.FirstOrDefault(lesson =>
            lesson.Id != ignoredPrivateLessonId
            && dates.Any(date => lesson.OverlapsWith(date, schedule.StartTime, schedule.DurationMinutes)));

        return overlappingLesson is null
            ? null
            : Busy(overlappingLesson.Date, PrivateLessonErrorCodes.ConflictingPrivateLessonIdDetail, overlappingLesson.Id);
    }

    public static async Task<ResultError?> FindWeeklyPrivateLessonConflictAsync(
        IPrivateLessonRepository privateLessonRepository,
        Guid instructorId,
        ClassSchedule schedule,
        DateOnly today,
        CancellationToken cancellationToken)
    {
        var upcomingLessons = await privateLessonRepository.ListByInstructorFromAsync(instructorId, today, cancellationToken);
        var overlappingLesson = upcomingLessons.FirstOrDefault(lesson =>
            schedule.MeetsOn(lesson.Date.DayOfWeek)
            && lesson.OverlapsWith(lesson.Date, schedule.StartTime, schedule.DurationMinutes));

        return overlappingLesson is null
            ? null
            : Busy(overlappingLesson.Date, PrivateLessonErrorCodes.ConflictingPrivateLessonIdDetail, overlappingLesson.Id);
    }

    private static ResultError Busy(DateOnly date, string conflictDetail, Guid conflictingId) =>
        new(ClassGroupErrorCodes.InstructorBusy, InstructorBusyMessage, ErrorKind.Conflict)
        {
            Details = new Dictionary<string, object?>
            {
                [PrivateLessonErrorCodes.ConflictingDateDetail] = date,
                [conflictDetail] = conflictingId,
            },
        };
}

internal sealed record ClassGroupOverlap(DateOnly Date, ClassGroup ClassGroup);

internal sealed record InstructorAgendaRepositories(
    IClassGroupRepository ClassGroups,
    IClassSessionRepository Sessions,
    IPrivateLessonRepository PrivateLessons);
