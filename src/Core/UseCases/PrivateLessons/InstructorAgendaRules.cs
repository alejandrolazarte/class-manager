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

        var classGroups = await repositories.ClassGroups.ListActiveByInstructorAsync(instructorId, cancellationToken);
        if (classGroups.Count == 0)
        {
            return null;
        }

        var sessions = (await repositories.Sessions.ListBetweenAsync(dates.Min(), dates.Max(), cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        foreach (var date in dates)
        {
            var lessonTime = ClassSchedule.ForDay(date.DayOfWeek, schedule.StartTime, schedule.DurationMinutes);
            var overlappingClassGroup = classGroups
                .Where(classGroup => classGroup.Schedule.MeetsOn(date.DayOfWeek))
                .FirstOrDefault(classGroup =>
                {
                    var session = sessions.GetValueOrDefault((classGroup.Id, date));
                    if (session?.IsCancelled == true)
                    {
                        return false;
                    }

                    var startTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
                    return ClassSchedule.ForDay(date.DayOfWeek, startTime, classGroup.DurationMinutes).OverlapsWith(lessonTime);
                });
            if (overlappingClassGroup is not null)
            {
                return Busy(date, ClassGroupErrorCodes.ConflictingClassGroupIdDetail, overlappingClassGroup.Id);
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

internal sealed record InstructorAgendaRepositories(
    IClassGroupRepository ClassGroups,
    IClassSessionRepository Sessions,
    IPrivateLessonRepository PrivateLessons);
