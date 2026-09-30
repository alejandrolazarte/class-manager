using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record GetSessionQuery(Guid ClassGroupId, DateOnly Date);

public sealed record SessionStudentResponse(
    Guid StudentId,
    string StudentFullName,
    string ClientFullName,
    DateOnly? BirthDate,
    AttendanceStatus? Status,
    string? Feedback);

public sealed record SessionDetailsResponse(
    Guid ClassGroupId,
    string ClassGroupName,
    Guid InstructorId,
    string InstructorFullName,
    string? OriginalInstructorFullName,
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
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IClassFeedbackRepository feedbackRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
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
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Sessions.ViewAll, cancellationToken);
        if (!SessionRules.IsInScope(scope, classGroup.Value!, session))
        {
            return SessionRules.ClassGroupNotFound();
        }

        var usualInstructor = await instructorRepository.GetByIdAsync(classGroup.Value!.InstructorId, cancellationToken);
        var substitute = session?.SubstituteInstructorId is { } substituteInstructorId
            ? await instructorRepository.GetByIdAsync(substituteInstructorId, cancellationToken)
            : null;
        var instructor = substitute ?? usualInstructor;
        var statuses = session is null
            ? new Dictionary<Guid, AttendanceStatus>()
            : (await attendanceRepository.ListBySessionAsync(session.Id, cancellationToken))
                .ToDictionary(attendance => attendance.StudentId, attendance => attendance.Status);
        var feedbacks = session is null
            ? new Dictionary<Guid, string>()
            : (await feedbackRepository.ListBySessionAsync(session.Id, cancellationToken))
                .ToDictionary(feedback => feedback.StudentId, feedback => feedback.Text);
        var roster = await enrollmentRepository.ListRosterOnAsync(command.ClassGroupId, command.Date, cancellationToken);
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var isCancelled = session?.IsCancelled ?? false;
        var usualStartTime = classGroup.Value!.StartTime;
        var startTime = session?.EffectiveStartTime(usualStartTime) ?? usualStartTime;

        return new SessionDetailsResponse(
            classGroup.Value.Id,
            classGroup.Value.Name,
            session?.EffectiveInstructorId(classGroup.Value.InstructorId) ?? classGroup.Value.InstructorId,
            instructor?.FullName ?? string.Empty,
            substitute is null ? null : usualInstructor?.FullName,
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
                    statuses.TryGetValue(entry.StudentId, out var status) ? status : null,
                    feedbacks.GetValueOrDefault(entry.StudentId))),
            ]);
    }

    private static string FormatTime(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
