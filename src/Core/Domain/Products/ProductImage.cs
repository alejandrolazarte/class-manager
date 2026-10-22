using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class ProductImage : ITenantOwned, IAuditable, ICatalogImageLink
{
    private ProductImage()
    {
    }

    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long ProductId { get; private set; }
    public long DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public int Position { get; private set; }

    public void MoveTo(int position) => Position = position;

    internal static ProductImage Create(long productId, Document document, int position) =>
        new()
        {
            ProductId = productId,
            DocumentId = document.Id,
            Document = document,
            Position = position,
        };
}
