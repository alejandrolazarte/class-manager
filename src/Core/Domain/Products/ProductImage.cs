using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class ProductImage : ITenantOwned, ICatalogImageLink
{
    private ProductImage()
    {
    }

    public Guid TenantId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid DocumentId { get; private set; }
    public Document Document { get; private set; } = null!;
    public int Position { get; private set; }

    public void MoveTo(int position) => Position = position;

    internal static ProductImage Create(Guid productId, Document document, int position) =>
        new()
        {
            ProductId = productId,
            DocumentId = document.Id,
            Document = document,
            Position = position,
        };
}
