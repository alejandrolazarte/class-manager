using ClassManager.Core.Domain.Sessions;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.PrivateLessons;

public sealed class PrivateLessonStudent : ITenantOwned
{
    private PrivateLessonStudent()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid PrivateLessonId { get; private set; }
    public Guid StudentId { get; private set; }
    public AttendanceStatus? Status { get; private set; }

    internal static PrivateLessonStudent Create(Guid privateLessonId, Guid studentId) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            PrivateLessonId = privateLessonId,
            StudentId = studentId,
        };

    internal void Mark(AttendanceStatus? status) => Status = status;
}
