using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.PrivateLessons;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RescheduleSessionRequest(string? StartTime);

public sealed record RescheduleSessionCommand(Guid ClassGroupId, DateOnly Date, string? StartTime);

public sealed class RescheduleSessionUseCase(
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IPrivateLessonRepository privateLessonRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<RescheduleSessionCommand, SessionStatusResponse>
{
    private const string InPastMessage = "Past dates can't be rescheduled.";
    private const string StartTimeFormatMessage = "Start time must use the HH:mm format.";
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

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        var instructorId = session?.EffectiveInstructorId(classGroup.Value!.InstructorId) ?? classGroup.Value!.InstructorId;
        var conflict = await SessionRules.FindInstructorConflictAsync(
            new InstructorAgendaRepositories(classGroupRepository, sessionRepository, privateLessonRepository),
            instructorId,
            classGroup.Value!,
            command.Date,
            startTime,
            cancellationToken);
        if (conflict is not null)
        {
            return conflict;
        }

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
}
