using ClassManager.Core.Domain.Sessions;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.PrivateLessons;

public sealed class PrivateLessonStudent : ITenantOwned, IAuditable
{
    private PrivateLessonStudent()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
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
