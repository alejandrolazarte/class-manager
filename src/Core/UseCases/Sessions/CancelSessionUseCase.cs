using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record CancelSessionRequest(string? Reason);

public sealed record CancelSessionCommand(Guid ClassGroupId, DateOnly Date, string? Reason);

public sealed record SessionStatusResponse(Guid ClassGroupId, DateOnly Date, bool IsCancelled, string? CancellationReason);

public sealed class CancelSessionUseCase(
    IClassGroupRepository classGroupRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<CancelSessionCommand, SessionStatusResponse>
{
    private const string HasAttendanceMessage = "Attendance was already taken for this class.";
    private const string ConcurrentUpdateMessage = "The session changed at the same time. Try again.";

    public async Task<Result<SessionStatusResponse>> ExecuteAsync(CancelSessionCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, timeProvider.GetUtcNow());
            sessionRepository.Add(session);
        }
        else if ((await attendanceRepository.ListBySessionAsync(session.Id, cancellationToken)).Count > 0)
        {
            return Result.Conflict<SessionStatusResponse>(HasAttendanceMessage, SessionErrorCodes.HasAttendance);
        }

        var cancel = session.Cancel(command.Reason, timeProvider.GetUtcNow());
        if (cancel.IsFailure)
        {
            return cancel.Error!;
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
