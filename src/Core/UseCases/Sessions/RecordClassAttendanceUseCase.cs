using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record StudentAttendance(long StudentId, AttendanceStatus? Status);

public sealed record RecordClassAttendanceRequest(IReadOnlyList<StudentAttendance>? Students);

public sealed record RecordClassAttendanceCommand(long ClassGroupId, DateOnly Date, IReadOnlyList<StudentAttendance>? Students) : ICommand;

public sealed class RecordClassAttendanceUseCase(
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
    : IUseCase<RecordClassAttendanceCommand, IReadOnlyList<RecordAttendanceResponse>>
{
    public const int MaximumStudents = 200;

    private const string NoStudentsMessage = "Choose at least one student.";
    private const string TooManyStudentsMessage = "At most 200 students can be marked at once.";
    private const string RepeatedStudentMessage = "Each student can appear only once.";
    private const string InFutureMessage = "Attendance can't be taken before the class date.";
    private const string StudentNotEnrolledMessage = "A student isn't enrolled in this class on that date.";
    private const string CancelledMessage = "The class is cancelled on that date.";
    private const string ConcurrentUpdateMessage = "The session changed at the same time. Try again.";

    public async Task<Result<IReadOnlyList<RecordAttendanceResponse>>> ExecuteAsync(RecordClassAttendanceCommand command, CancellationToken cancellationToken)
    {
        var students = command.Students ?? [];
        if (Validate(students) is { } invalid)
        {
            return invalid;
        }

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
            return Result.Validation<IReadOnlyList<RecordAttendanceResponse>>(InFutureMessage, SessionErrorCodes.InFuture, nameof(RecordClassAttendanceCommand.Date));
        }

        var studentIds = students.Select(student => student.StudentId).ToList();
        if (!await SessionRules.AreAllInClassAsync(
            enrollmentRepository, makeupBookingRepository, packBookingRepository, command.ClassGroupId, command.Date, session, studentIds, cancellationToken))
        {
            return Result.Validation<IReadOnlyList<RecordAttendanceResponse>>(
                StudentNotEnrolledMessage, SessionErrorCodes.StudentNotEnrolled, nameof(RecordClassAttendanceCommand.Students));
        }

        if (session?.IsCancelled == true)
        {
            return Result.Conflict<IReadOnlyList<RecordAttendanceResponse>>(CancelledMessage, SessionErrorCodes.Cancelled);
        }

        if (session is null)
        {
            session = ClassSession.Create(command.ClassGroupId, command.Date, timeProvider.GetUtcNow());
            sessionRepository.Add(session);
        }

        foreach (var student in students)
        {
            await AttendanceMarks.ApplyAsync(attendanceRepository, session, student.StudentId, student.Status, cancellationToken);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return Result.Conflict<IReadOnlyList<RecordAttendanceResponse>>(ConcurrentUpdateMessage, SessionErrorCodes.ConcurrentUpdate);
        }

        return students.Select(student => new RecordAttendanceResponse(student.StudentId, student.Status)).ToList();
    }

    private static Result<IReadOnlyList<RecordAttendanceResponse>>? Validate(IReadOnlyList<StudentAttendance> students)
    {
        if (students.Count == 0)
        {
            return Result.Validation<IReadOnlyList<RecordAttendanceResponse>>(NoStudentsMessage, fieldName: nameof(RecordClassAttendanceCommand.Students));
        }

        if (students.Count > MaximumStudents)
        {
            return Result.Validation<IReadOnlyList<RecordAttendanceResponse>>(TooManyStudentsMessage, fieldName: nameof(RecordClassAttendanceCommand.Students));
        }

        return students.Select(student => student.StudentId).Distinct().Count() != students.Count
            ? Result.Validation<IReadOnlyList<RecordAttendanceResponse>>(RepeatedStudentMessage, fieldName: nameof(RecordClassAttendanceCommand.Students))
            : null;
    }
}
