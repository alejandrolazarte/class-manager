using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPackPurchaseClassGroup : ITenantOwned, IAuditable
{
    private ClassPackPurchaseClassGroup()
    {
    }

    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long ClassPackPurchaseId { get; private set; }
    public long ClassGroupId { get; private set; }

    public static ClassPackPurchaseClassGroup Create(long classPackPurchaseId, long classGroupId) =>
        new()
        {
            ClassPackPurchaseId = classPackPurchaseId,
            ClassGroupId = classGroupId,
        };
}
