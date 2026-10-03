using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record ListDaySessionsQuery(DateOnly? Date);

public enum SessionKind
{
    Group,
    Private,
}

public sealed record DaySessionResponse(
    SessionKind Kind,
    Guid? ClassGroupId,
    Guid? PrivateLessonId,
    string ClassGroupName,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string? OriginalStartTime,
    string InstructorFullName,
    string? Location,
    bool IsCancelled,
    string? CancellationReason,
    int EnrolledCount,
    int PresentCount,
    int AbsentCount,
    IReadOnlyList<string> StudentNames,
    bool IsTrial = false,
    string? OriginalInstructorFullName = null);

public sealed class ListDaySessionsUseCase(
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IMakeupBookingRepository makeupBookingRepository,
    IPackBookingRepository packBookingRepository,
    IPrivateLessonRepository privateLessonRepository,
    IStudentRepository studentRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<ListDaySessionsQuery, IReadOnlyList<DaySessionResponse>>
{
    private const string StudentNameSeparator = ", ";

    private static readonly AttendanceCount NoAttendance = new(0, 0);

    public async Task<Result<IReadOnlyList<DaySessionResponse>>> ExecuteAsync(ListDaySessionsQuery command, CancellationToken cancellationToken)
    {
        var date = command.Date ?? await businessCalendar.TodayAsync(cancellationToken);
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Sessions.ViewAll, cancellationToken);
        var sessions = (await sessionRepository.ListByDateAsync(date, cancellationToken))
            .ToDictionary(session => session.ClassGroupId);
        var classGroups = (await classGroupRepository.ListActiveAsync(cancellationToken))
            .Where(classGroup => classGroup.Schedule.MeetsOn(date.DayOfWeek)
                && SessionRules.IsInScope(scope, classGroup, sessions.GetValueOrDefault(classGroup.Id)))
            .ToList();
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);
        var enrolledCounts = await enrollmentRepository.CountActiveOnByClassGroupAsync(date, cancellationToken);
        IReadOnlyCollection<Guid> sessionIds = [.. sessions.Values.Select(session => session.Id)];
        var attendanceCounts = await attendanceRepository.CountBySessionsAsync(sessionIds, cancellationToken);
        var makeupCounts = await makeupBookingRepository.CountBySessionsAsync(sessionIds, cancellationToken);
        var packBookingCounts = await packBookingRepository.CountBySessionsAsync(sessionIds, cancellationToken);

        var privateLessons = (await privateLessonRepository.ListBetweenAsync(date, date, cancellationToken))
            .Where(lesson => scope.Includes(lesson.InstructorId))
            .ToList();
        var studentNames = (await studentRepository.ListSummariesByIdsAsync(
                [.. privateLessons.SelectMany(lesson => lesson.Students).Select(lessonStudent => lessonStudent.StudentId).Distinct()],
                cancellationToken))
            .ToDictionary(student => student.Id, student => student.FullName);

        return Result.Success(SortByStartTime(
        [
            .. privateLessons.Select(lesson =>
            {
                IReadOnlyList<string> names =
                [
                    .. lesson.Students
                        .Select(lessonStudent => studentNames.GetValueOrDefault(lessonStudent.StudentId, string.Empty))
                        .Order(StringComparer.CurrentCultureIgnoreCase),
                ];
                return new DaySessionResponse(
                    SessionKind.Private,
                    null,
                    lesson.Id,
                    string.Join(StudentNameSeparator, names),
                    date,
                    FormatTime(lesson.StartTime),
                    FormatTime(lesson.EndTime),
                    null,
                    instructorNames.GetValueOrDefault(lesson.InstructorId, string.Empty),
                    lesson.Location,
                    lesson.IsCancelled,
                    lesson.CancellationReason,
                    lesson.Students.Count,
                    lesson.Students.Count(lessonStudent => lessonStudent.Status == AttendanceStatus.Present),
                    lesson.Students.Count(lessonStudent => lessonStudent.Status == AttendanceStatus.Absent),
                    names,
                    lesson.IsTrial);
            }),
            .. classGroups.Select(classGroup =>
            {
                var session = sessions.GetValueOrDefault(classGroup.Id);
                var attendanceCount = session is null ? NoAttendance : attendanceCounts.GetValueOrDefault(session.Id, NoAttendance);
                var startTime = session?.EffectiveStartTime(classGroup.StartTime) ?? classGroup.StartTime;
                return new DaySessionResponse(
                    SessionKind.Group,
                    classGroup.Id,
                    null,
                    classGroup.Name,
                    date,
                    FormatTime(startTime),
                    FormatTime(startTime.AddMinutes(classGroup.DurationMinutes)),
                    session?.RescheduledStartTime is null ? null : FormatTime(classGroup.StartTime),
                    instructorNames.GetValueOrDefault(session?.EffectiveInstructorId(classGroup.InstructorId) ?? classGroup.InstructorId, string.Empty),
                    classGroup.Location,
                    session?.IsCancelled ?? false,
                    session?.CancellationReason,
                    enrolledCounts.GetValueOrDefault(classGroup.Id)
                        + (session is null ? 0 : makeupCounts.GetValueOrDefault(session.Id) + packBookingCounts.GetValueOrDefault(session.Id)),
                    attendanceCount.Present,
                    attendanceCount.Absent,
                    [],
                    OriginalInstructorFullName: session?.SubstituteInstructorId is null
                        ? null
                        : instructorNames.GetValueOrDefault(classGroup.InstructorId, string.Empty));
            }),
        ]));
    }

    private static IReadOnlyList<DaySessionResponse> SortByStartTime(IEnumerable<DaySessionResponse> sessions) =>
    [
        .. sessions
            .OrderBy(session => session.StartTime, StringComparer.Ordinal)
            .ThenBy(session => session.ClassGroupName, StringComparer.CurrentCultureIgnoreCase),
    ];

    private static string FormatTime(TimeOnly time) => time.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture);
}
