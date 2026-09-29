using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class ProductVariant : ITenantOwned
{
    public const int NameMaxLength = 30;

    private ProductVariant()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public bool IsActive { get; private set; }

    internal static ProductVariant Create(Guid productId, string name, int position) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ProductId = productId,
            Name = name,
            Position = position,
            IsActive = true,
        };

    internal void Rename(string name, int position)
    {
        Name = name;
        Position = position;
        IsActive = true;
    }

    internal void Deactivate() => IsActive = false;
}
