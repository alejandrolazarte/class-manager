using ClassManager.Core.Domain.Sessions;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.PrivateLessons;

public sealed class PrivateLessonStudent : ITenantOwned, IAuditable
{
    private PrivateLessonStudent()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long PrivateLessonId { get; private set; }
    public long StudentId { get; private set; }
    public AttendanceStatus? Status { get; private set; }

    internal static PrivateLessonStudent Create(long privateLessonId, long studentId) =>
        new()
        {
            PrivateLessonId = privateLessonId,
            StudentId = studentId,
        };

    internal void Mark(AttendanceStatus? status) => Status = status;
}
