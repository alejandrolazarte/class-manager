using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class ProductVariant : ITenantOwned
{
    public const int SizeMaxLength = 30;

    private const string LabelSeparator = " · ";

    private ProductVariant()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProductId { get; private set; }
    public string Size { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public int Position { get; private set; }
    public bool IsActive { get; private set; }

    public string Label => string.Join(
        LabelSeparator,
        new[] { Size, Color is null ? string.Empty : ProductColors.NameOf(Color) }.Where(part => part.Length > 0));

    internal static ProductVariant Create(Guid productId, string size, string? color, int position) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ProductId = productId,
            Size = size,
            Color = color,
            Position = position,
            IsActive = true,
        };

    internal void Change(string size, string? color, int position)
    {
        Size = size;
        Color = color;
        Position = position;
        IsActive = true;
    }

    internal void Deactivate() => IsActive = false;
}
