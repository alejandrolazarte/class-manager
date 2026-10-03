using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record RecordAttendanceRequest(AttendanceStatus? Status);

public sealed record RecordAttendanceCommand(Guid ClassGroupId, DateOnly Date, Guid StudentId, AttendanceStatus? Status);

public sealed record RecordAttendanceResponse(Guid StudentId, AttendanceStatus? Status);

public sealed class RecordAttendanceUseCase(
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IMakeupBookingRepository makeupBookingRepository,
    IPackBookingRepository packBookingRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IUnitOfWork unitOfWork,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider,
    IAccessScopes accessScopes)
    : IUseCase<RecordAttendanceCommand, RecordAttendanceResponse>
{
    private const string InFutureMessage = "Attendance can't be taken before the class date.";
    private const string StudentNotEnrolledMessage = "The student isn't enrolled in this class on that date.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string ConcurrentUpdateMessage = "The session changed at the same time. Try again.";

    public async Task<Result<RecordAttendanceResponse>> ExecuteAsync(RecordAttendanceCommand command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Attendance.RecordAll, cancellationToken);
        if (!SessionRules.IsInScope(scope, classGroup.Value!, session))
        {
            return AccessRules.NotYours();
        }

        if (command.Date > await businessCalendar.TodayAsync(cancellationToken))
        {
            return Result.Validation<RecordAttendanceResponse>(InFutureMessage, SessionErrorCodes.InFuture, nameof(RecordAttendanceCommand.Date));
        }

        if (!await SessionRules.IsInClassAsync(
            enrollmentRepository, makeupBookingRepository, packBookingRepository, command.ClassGroupId, command.Date, session, command.StudentId, cancellationToken))
        {
            return Result.Validation<RecordAttendanceResponse>(
                StudentNotEnrolledMessage, SessionErrorCodes.StudentNotEnrolled, nameof(RecordAttendanceCommand.StudentId));
        }

        if (session?.IsCancelled == true)
        {
            return Result.Conflict<RecordAttendanceResponse>(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, timeProvider.GetUtcNow());
            sessionRepository.Add(session);
        }

        var attendance = await attendanceRepository.FindForUpdateAsync(session.Id, command.StudentId, cancellationToken);
        if (command.Status is null)
        {
            if (attendance is not null)
            {
                attendanceRepository.Remove(attendance);
            }
        }
        else if (attendance is null)
        {
            attendanceRepository.Add(Attendance.Create(session.Id, command.StudentId, command.Status.Value));
        }
        else
        {
            attendance.ChangeStatus(command.Status.Value);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<RecordAttendanceResponse>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        return new RecordAttendanceResponse(command.StudentId, command.Status);
    }
}
