using ClassManager.Core.Domain.Documents;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassGroups;

public sealed class ClassGroupMaterialFile : ITenantOwned, IAuditable
{
    private ClassGroupMaterialFile()
    {
    }

    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long ClassGroupId { get; private set; }
    public long DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public int Position { get; private set; }

    internal static ClassGroupMaterialFile Create(long classGroupId, Document document, int position) =>
        new()
        {
            ClassGroupId = classGroupId,
            DocumentId = document.Id,
            Document = document,
            Position = position,
        };
}
