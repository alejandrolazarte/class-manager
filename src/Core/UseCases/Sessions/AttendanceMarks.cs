using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Core.UseCases.Sessions;

internal static class AttendanceMarks
{
    public static async Task ApplyAsync(
        IAttendanceRepository attendanceRepository,
        ClassSession session,
        long studentId,
        AttendanceStatus? status,
        CancellationToken cancellationToken)
    {
        var attendance = await attendanceRepository.FindForUpdateAsync(session.Id, studentId, cancellationToken);
        Apply(attendanceRepository, session, studentId, status, attendance);
    }

    public static void Apply(
        IAttendanceRepository attendanceRepository,
        ClassSession session,
        long studentId,
        AttendanceStatus? status,
        Attendance? attendance)
    {
        if (status is null)
        {
            if (attendance is not null)
            {
                attendanceRepository.Remove(attendance);
            }
        }
        else if (attendance is null)
        {
            attendanceRepository.Add(Attendance.Create(session, studentId, status.Value));
        }
        else
        {
            attendance.ChangeStatus(status.Value);
        }
    }
}
