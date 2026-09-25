using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class Attendance : ITenantOwned
{
    private Attendance()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClassSessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public AttendanceStatus Status { get; private set; }

    public static Attendance Create(Guid classSessionId, Guid studentId, AttendanceStatus status) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ClassSessionId = classSessionId,
            StudentId = studentId,
            Status = status,
        };

    public void ChangeStatus(AttendanceStatus status) => Status = status;
}
