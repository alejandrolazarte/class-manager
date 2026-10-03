using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPackImage : ITenantOwned, ICatalogImageLink
{
    private ClassPackImage()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid ClassPackId { get; private set; }
    public Guid DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public int Position { get; private set; }

    public void MoveTo(int position) => Position = position;

    internal static ClassPackImage Create(Guid classPackId, Document document, int position) =>
        new()
        {
            ClassPackId = classPackId,
            DocumentId = document.Id,
            Document = document,
            Position = position,
        };
}
