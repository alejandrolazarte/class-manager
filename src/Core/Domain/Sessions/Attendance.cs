using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class Attendance : ITenantOwned, IAuditable
{
    private Attendance()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long ClassSessionId { get; private set; }
    public ClassSession? Session { get; private set; }
    public long StudentId { get; private set; }
    public AttendanceStatus Status { get; private set; }

    public static Attendance Create(ClassSession session, long studentId, AttendanceStatus status) =>
        new()
        {
            ClassSessionId = session.Id,
            Session = session,
            StudentId = studentId,
            Status = status,
        };

    public void ChangeStatus(AttendanceStatus status) => Status = status;
}
