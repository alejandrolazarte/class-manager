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
        Guid classGroupId,
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

    public static bool IsInScope(InstructorScope scope, ClassGroup classGroup, ClassSession? session) =>
        scope.Includes(classGroup.InstructorId)
        || (session?.SubstituteInstructorId is { } substituteInstructorId && scope.Includes(substituteInstructorId));

    public static async Task<ResultError?> FindInstructorConflictAsync(
        InstructorAgendaRepositories repositories,
        Guid instructorId,
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
