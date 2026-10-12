using ClassManager.Core.Domain.Documents;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassGroups;

public sealed class ClassGroupMaterialFile : ITenantOwned
{
    private ClassGroupMaterialFile()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid ClassGroupId { get; private set; }
    public Guid DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public int Position { get; private set; }

    internal static ClassGroupMaterialFile Create(Guid classGroupId, Document document, int position) =>
        new()
        {
            ClassGroupId = classGroupId,
            DocumentId = document.Id,
            Document = document,
            Position = position,
        };
}
