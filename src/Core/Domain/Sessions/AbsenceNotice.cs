using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Sessions;

public sealed class AbsenceNotice : ITenantOwned
{
    private AbsenceNotice()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClassSessionId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static AbsenceNotice Create(Guid classSessionId, Guid studentId, DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ClassSessionId = classSessionId,
            StudentId = studentId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
