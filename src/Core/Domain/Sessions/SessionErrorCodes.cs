namespace ClassManager.Core.Domain.Sessions;

public static class SessionErrorCodes
{
    public const string NotScheduled = "session.not_scheduled";
    public const string InFuture = "session.in_future";
    public const string InPast = "session.in_past";
    public const string Cancelled = "session.cancelled";
    public const string HasAttendance = "session.has_attendance";
    public const string ConcurrentUpdate = "session.concurrent_update";
    public const string StudentNotEnrolled = "attendance.student_not_enrolled";
}
