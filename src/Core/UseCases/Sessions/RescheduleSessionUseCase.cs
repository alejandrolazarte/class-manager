using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RescheduleSessionRequest(string? StartTime);

public sealed record RescheduleSessionCommand(Guid ClassGroupId, DateOnly Date, string? StartTime);

public sealed class RescheduleSessionUseCase(
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<RescheduleSessionCommand, SessionStatusResponse>
{
    private const string InPastMessage = "Past dates can't be rescheduled.";
    private const string StartTimeFormatMessage = "Start time must use the HH:mm format.";
    private const string InstructorBusyMessage = "The instructor already teaches another class group at that time.";
    private const string ConcurrentUpdateMessage = "The session changed at the same time. Try again.";

    public async Task<Result<SessionStatusResponse>> ExecuteAsync(RescheduleSessionCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        if (command.Date < await businessCalendar.TodayAsync(cancellationToken))
        {
            return Result.Validation<SessionStatusResponse>(InPastMessage, SessionErrorCodes.InPast, nameof(RescheduleSessionCommand.Date));
        }

        if (!TimeOnly.TryParseExact(command.StartTime?.Trim(), ClassSchedule.TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var startTime))
        {
            return Result.Validation<SessionStatusResponse>(StartTimeFormatMessage, fieldName: nameof(RescheduleSessionCommand.StartTime));
        }

        var conflict = await FindInstructorConflictAsync(classGroup.Value!, command.Date, startTime, cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, timeProvider.GetUtcNow());
            sessionRepository.Add(session);
        }

        var reschedule = session.Reschedule(startTime, classGroup.Value!.DurationMinutes, classGroup.Value.StartTime);
        if (reschedule.IsFailure)
        {
            return reschedule.Error!;
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<SessionStatusResponse>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        return new SessionStatusResponse(session.ClassGroupId, session.Date, session.IsCancelled, session.CancellationReason);
    }

    private async Task<ResultError?> FindInstructorConflictAsync(
        ClassGroup classGroup,
        DateOnly sessionDate,
        TimeOnly startTime,
        CancellationToken cancellationToken)
    {
        var newTime = ClassSchedule.ForDay(sessionDate.DayOfWeek, startTime, classGroup.DurationMinutes);
        var sessionsOfDay = (await sessionRepository.ListByDateAsync(sessionDate, cancellationToken))
            .ToDictionary(session => session.ClassGroupId);
        var instructorClassGroups = await classGroupRepository.ListActiveByInstructorAsync(classGroup.InstructorId, cancellationToken);

        var overlappingClassGroup = instructorClassGroups
            .Where(other => other.Id != classGroup.Id && other.Schedule.MeetsOn(sessionDate.DayOfWeek))
            .FirstOrDefault(other =>
            {
                var otherSession = sessionsOfDay.GetValueOrDefault(other.Id);
                if (otherSession?.IsCancelled == true)
                {
                    return false;
                }

                var otherStartTime = otherSession?.EffectiveStartTime(other.StartTime) ?? other.StartTime;
                return ClassSchedule.ForDay(sessionDate.DayOfWeek, otherStartTime, other.DurationMinutes).OverlapsWith(newTime);
            });

        return overlappingClassGroup is null
            ? null
            : new ResultError(ClassGroupErrorCodes.InstructorBusy, InstructorBusyMessage, ErrorKind.Conflict)
            {
                Details = new Dictionary<string, object?> { [ClassGroupErrorCodes.ConflictingClassGroupIdDetail] = overlappingClassGroup.Id },
            };
    }
}
