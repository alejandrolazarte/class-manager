using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record ListMonthCalendarQuery(string? Month);

public sealed record CalendarDayResponse(DateOnly Date, int ClassCount, int CancelledCount, int PendingAttendanceCount);

public sealed record MonthCalendarResponse(string Month, IReadOnlyList<CalendarDayResponse> Days);

public sealed class ListMonthCalendarUseCase(
    IClassGroupRepository classGroupRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IPrivateLessonRepository privateLessonRepository,
    IBusinessCalendarService businessCalendar,
    IAccessScopes accessScopes)
    : IUseCase<ListMonthCalendarQuery, MonthCalendarResponse>
{
    private static readonly AttendanceCount NoAttendance = new(0, 0);

    public async Task<Result<MonthCalendarResponse>> ExecuteAsync(ListMonthCalendarQuery command, CancellationToken cancellationToken)
    {
        var today = await businessCalendar.TodayAsync(cancellationToken);
        var month = command.Month is null
            ? BillingMonth.From(today)
            : BillingMonth.Parse(command.Month, nameof(ListMonthCalendarQuery.Month));
        if (month.IsFailure)
        {
            return month.Error!;
        }

        var firstDay = month.Value!.FirstDay;
        var lastDay = month.Value.LastDay;
        var scope = await accessScopes.ForInstructorsAsync(Permissions.Sessions.ViewAll, cancellationToken);
        var classGroups = await classGroupRepository.ListActiveAsync(cancellationToken);
        var enrollmentPeriods = await enrollmentRepository.ListActiveInPeriodAsync(firstDay, lastDay, cancellationToken);
        var sessions = (await sessionRepository.ListBetweenAsync(firstDay, lastDay, cancellationToken))
            .ToDictionary(session => (session.ClassGroupId, session.Date));
        var attendanceCounts = await attendanceRepository.CountBySessionsAsync(
            [.. sessions.Values.Select(session => session.Id)], cancellationToken);
        var privateLessonsByDate = (await privateLessonRepository.ListBetweenAsync(firstDay, lastDay, cancellationToken))
            .Where(lesson => scope.Includes(lesson.InstructorId))
            .ToLookup(lesson => lesson.Date);

        var days = new List<CalendarDayResponse>();
        for (var date = firstDay; date <= lastDay; date = date.AddDays(1))
        {
            var dayClassGroups = classGroups
                .Where(classGroup => classGroup.Schedule.MeetsOn(date.DayOfWeek)
                    && SessionRules.IsInScope(scope, classGroup, sessions.GetValueOrDefault((classGroup.Id, date))))
                .ToList();
            var dayPrivateLessons = privateLessonsByDate[date].ToList();
            if (dayClassGroups.Count == 0 && dayPrivateLessons.Count == 0)
            {
                continue;
            }

            var cancelledCount = dayPrivateLessons.Count(lesson => lesson.IsCancelled);
            var pendingAttendanceCount = date < today
                ? dayPrivateLessons.Count(lesson => !lesson.IsCancelled && lesson.Students.Any(lessonStudent => lessonStudent.Status is null))
                : 0;
            foreach (var classGroup in dayClassGroups)
            {
                var session = sessions.GetValueOrDefault((classGroup.Id, date));
                if (session?.IsCancelled == true)
                {
                    cancelledCount++;
                    continue;
                }

                var enrolledCount = enrollmentPeriods.Count(period => period.ClassGroupId == classGroup.Id && period.IsActiveOn(date));
                var attendanceCount = session is null ? NoAttendance : attendanceCounts.GetValueOrDefault(session.Id, NoAttendance);
                if (date < today && attendanceCount.Present + attendanceCount.Absent < enrolledCount)
                {
                    pendingAttendanceCount++;
                }
            }

            days.Add(new CalendarDayResponse(date, dayClassGroups.Count + dayPrivateLessons.Count, cancelledCount, pendingAttendanceCount));
        }

        return new MonthCalendarResponse(month.Value.ToString(), days);
    }
}
