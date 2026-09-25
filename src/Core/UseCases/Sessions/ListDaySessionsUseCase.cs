using System.Globalization;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassGroups;

namespace ClassManager.Core.UseCases.Sessions;

public sealed record ListDaySessionsQuery(DateOnly? Date);

public sealed record DaySessionResponse(
    Guid ClassGroupId,
    string ClassGroupName,
    DateOnly Date,
    string StartTime,
    string EndTime,
    string InstructorFullName,
    string? Location,
    bool IsCancelled,
    string? CancellationReason,
    int EnrolledCount,
    int PresentCount,
    int AbsentCount);

public sealed class ListDaySessionsUseCase(
    IClassGroupRepository classGroupRepository,
    IInstructorRepository instructorRepository,
    IEnrollmentRepository enrollmentRepository,
    IClassSessionRepository sessionRepository,
    IAttendanceRepository attendanceRepository,
    IBusinessCalendarService businessCalendar)
    : IUseCase<ListDaySessionsQuery, IReadOnlyList<DaySessionResponse>>
{
    private static readonly AttendanceCount NoAttendance = new(0, 0);

    public async Task<Result<IReadOnlyList<DaySessionResponse>>> ExecuteAsync(ListDaySessionsQuery command, CancellationToken cancellationToken)
    {
        var date = command.Date ?? await businessCalendar.TodayAsync(cancellationToken);
        var classGroups = (await classGroupRepository.ListActiveAsync(cancellationToken))
            .Where(classGroup => classGroup.Schedule.MeetsOn(date.DayOfWeek))
            .OrderBy(classGroup => classGroup.StartTime)
            .ThenBy(classGroup => classGroup.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        var instructorNames = (await instructorRepository.ListAllAsync(cancellationToken))
            .ToDictionary(instructor => instructor.Id, instructor => instructor.FullName);
        var sessions = (await sessionRepository.ListByDateAsync(date, cancellationToken))
            .ToDictionary(session => session.ClassGroupId);
        var enrolledCounts = await enrollmentRepository.CountActiveOnByClassGroupAsync(date, cancellationToken);
        var attendanceCounts = await attendanceRepository.CountBySessionsAsync(
            [.. sessions.Values.Select(session => session.Id)], cancellationToken);

        return Result.Success<IReadOnlyList<DaySessionResponse>>(
        [
            .. classGroups.Select(classGroup =>
            {
                var session = sessions.GetValueOrDefault(classGroup.Id);
                var attendanceCount = session is null ? NoAttendance : attendanceCounts.GetValueOrDefault(session.Id, NoAttendance);
                var schedule = classGroup.Schedule;
                return new DaySessionResponse(
                    classGroup.Id,
                    classGroup.Name,
                    date,
                    schedule.StartTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
                    schedule.EndTime.ToString(ClassSchedule.TimeFormat, CultureInfo.InvariantCulture),
                    instructorNames.GetValueOrDefault(classGroup.InstructorId, string.Empty),
                    classGroup.Location,
                    session?.IsCancelled ?? false,
                    session?.CancellationReason,
                    enrolledCounts.GetValueOrDefault(classGroup.Id),
                    attendanceCount.Present,
                    attendanceCount.Absent);
            }),
        ]);
    }
}
