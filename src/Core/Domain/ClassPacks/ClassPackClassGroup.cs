using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPackClassGroup : ITenantOwned, IAuditable
{
    private ClassPackClassGroup()
    {
    }

    public Guid TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public Guid ClassPackId { get; private set; }
    public Guid ClassGroupId { get; private set; }

    public static ClassPackClassGroup Create(Guid classPackId, Guid classGroupId) =>
        new()
        {
            ClassPackId = classPackId,
            ClassGroupId = classGroupId,
        };
}
