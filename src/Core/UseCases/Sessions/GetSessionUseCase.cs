using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record GetSessionQuery(Guid ClassGroupId, DateOnly Date);

public sealed record SessionStudentResponse(
    Guid StudentId,
    string StudentFullName,
    string ClientFullName,
    DateOnly? BirthDate,
    AttendanceStatus? Status);

public sealed record SessionDetailsResponse(
    Guid ClassGroupId,
    string ClassGroupName,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? OriginalStartTime,
    bool IsCancelled,
    string? CancellationReason,
    bool CanTakeAttendance,
    bool CanReschedule,
    IReadOnlyList<SessionStudentResponse> Students);

public sealed class GetSessionUseCase(
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<GetSessionQuery, SessionDetailsResponse>
{
    public async Task<Result<SessionDetailsResponse>> ExecuteAsync(GetSessionQuery command, CancellationToken cancellationToken)
    {
        var classGroup = await SessionRules.FindScheduledClassGroupAsync(classGroupRepository, command.ClassGroupId, command.Date, cancellationToken);
        if (classGroup.IsFailure)
        {
            return classGroup.Error!;
        }

        var session = await sessionRepository.FindForUpdateAsync(command.ClassGroupId, command.Date, cancellationToken);
        var statuses = session is null
            ? new Dictionary<Guid, AttendanceStatus>()
            : (await attendanceRepository.ListBySessionAsync(session.Id, cancellationToken))
                .ToDictionary(attendance => attendance.StudentId, attendance => attendance.Status);
        var roster = await enrollmentRepository.ListRosterOnAsync(command.ClassGroupId, command.Date, cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var isCancelled = session?.IsCancelled ?? false;
        var usualStartTime = classGroup.Value!.StartTime;
        var startTime = session?.EffectiveStartTime(usualStartTime) ?? usualStartTime;

        return new SessionDetailsResponse(
            classGroup.Value.Id,
            classGroup.Value.Name,
            command.Date,
            FormatTime(startTime),
            FormatTime(startTime.AddMinutes(classGroup.Value.DurationMinutes)),
            session?.RescheduledStartTime is null ? null : FormatTime(usualStartTime),
            isCancelled,
            session?.CancellationReason,
            CanTakeAttendance: !isCancelled && command.Date <= today,
            CanReschedule: !isCancelled && command.Date >= today,
            [
                .. roster.Select(entry => new SessionStudentResponse(
                    entry.StudentId,
                    entry.StudentFullName,
                    entry.ClientFullName,
                    entry.BirthDate,
                    statuses.TryGetValue(entry.StudentId, out var status) ? status : null)),
            ]);
    }

    private static string FormatTime(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
