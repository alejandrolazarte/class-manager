using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class AbsenceNotice : ITenantOwned, IAuditable
{
    private AbsenceNotice()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClassSessionId { get; private set; }
    public ClassSession? Session { get; private set; }
    public long StudentId { get; private set; }
    public bool KeepsStreak { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    public static AbsenceNotice Create(ClassSession session, long studentId, bool keepsStreak, DateTimeOffset createdAt) =>
        new()
        {
            ClassSessionId = session.Id,
            Session = session,
            StudentId = studentId,
            KeepsStreak = keepsStreak,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
