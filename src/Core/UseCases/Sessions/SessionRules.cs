using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.UseCases.Sessions;

internal static class SessionRules
{
    private const string ClassGroupNotFoundMessage = "The class group does not exist.";
    private const string NotScheduledMessage = "The class group doesn't meet on that date.";
    private const string InstructorBusyMessage = "The instructor already teaches another class group at that time.";

    public static ResultError ClassGroupNotFound() => new(ClassGroupErrorCodes.NotFound, ClassGroupNotFoundMessage, ErrorKind.NotFound);

    public static async Task<Result<ClassGroup>> FindScheduledClassGroupAsync(
        IClassGroupRepository classGroupRepository,
        long classGroupId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var classGroup = await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken);
        if (classGroup is null)
        {
            return Result.NotFound<ClassGroup>(ClassGroupNotFoundMessage, ClassGroupErrorCodes.NotFound);
        }

        return classGroup.Schedule.MeetsOn(date.DayOfWeek)
            ? classGroup
            : Result.Validation<ClassGroup>(NotScheduledMessage, SessionErrorCodes.NotScheduled, nameof(ClassSession.Date));
    }

    public static async Task<bool> IsInClassAsync(
        IEnrollmentRepository enrollmentRepository,
        IMakeupBookingRepository makeupBookingRepository,
        IPackBookingRepository packBookingRepository,
        long classGroupId,
        DateOnly date,
        ClassSession? session,
        long studentId,
        CancellationToken cancellationToken)
    {
        var roster = await enrollmentRepository.ListRosterOnIncludingDeletedAsync(classGroupId, date, cancellationToken);
        if (roster.Any(entry => entry.StudentId == studentId))
        {
            return true;
        }

        return session is not null
            && ((await makeupBookingRepository.ListStudentsBySessionIncludingDeletedAsync(session.Id, cancellationToken)).Any(student => student.StudentId == studentId)
                || await packBookingRepository.FindForUpdateAsync(session.Id, studentId, cancellationToken) is not null);
    }

    public static async Task<bool> AreAllInClassAsync(
        IEnrollmentRepository enrollmentRepository,
        IMakeupBookingRepository makeupBookingRepository,
        IPackBookingRepository packBookingRepository,
        long classGroupId,
        DateOnly date,
        ClassSession? session,
        IReadOnlyCollection<long> studentIds,
        CancellationToken cancellationToken)
    {
        var roster = (await enrollmentRepository.ListRosterOnIncludingDeletedAsync(classGroupId, date, cancellationToken)).Select(entry => entry.StudentId).ToHashSet();
        var outsideRoster = studentIds.Where(studentId => !roster.Contains(studentId)).ToList();
        if (outsideRoster.Count == 0)
        {
            return true;
        }

        if (session is null)
        {
            return false;
        }

        var makeups = (await makeupBookingRepository.ListStudentsBySessionIncludingDeletedAsync(session.Id, cancellationToken)).Select(student => student.StudentId).ToHashSet();
        foreach (var studentId in outsideRoster.Where(studentId => !makeups.Contains(studentId)))
        {
            if (await packBookingRepository.FindForUpdateAsync(session.Id, studentId, cancellationToken) is null)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsListedOn(DateOnly date, DateOnly today, ClassSession? session, int enrolledCount) =>
        date >= today || session is not null || enrolledCount > 0;

    public static bool IsInScope(InstructorScope scope, ClassGroup classGroup, ClassSession? session) =>
        scope.Includes(classGroup.InstructorId)
        || (session?.SubstituteInstructorId is { } substituteInstructorId && scope.Includes(substituteInstructorId));

    public static async Task<ResultError?> FindInstructorConflictAsync(
        InstructorAgendaRepositories repositories,
        long instructorId,
        ClassGroup classGroup,
        DateOnly date,
        TimeOnly startTime,
        CancellationToken cancellationToken)
    {
        var overlap = await InstructorAgendaRules.FindClassGroupOverlapAsync(
            repositories, instructorId, [date], startTime, classGroup.DurationMinutes, classGroup.Id, cancellationToken);
        if (overlap is not null)
        {
            return new ResultError(ClassGroupErrorCodes.InstructorBusy, InstructorBusyMessage, ErrorKind.Conflict)
            {
                Details = new Dictionary<string, object?> { [ClassGroupErrorCodes.ConflictingClassGroupIdDetail] = overlap.ClassGroup.Id },
            };
        }

        return await InstructorAgendaRules.FindPrivateLessonConflictAsync(
            repositories.PrivateLessons,
            instructorId,
            [date],
            ClassSchedule.ForDay(date.DayOfWeek, startTime, classGroup.DurationMinutes),
            ignoredPrivateLessonId: null,
            cancellationToken);
    }
}
