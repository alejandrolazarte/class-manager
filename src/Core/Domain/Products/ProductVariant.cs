using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class ProductVariant : ITenantOwned, IAuditable
{
    public const int SizeMaxLength = 30;

    private const string LabelSeparator = " · ";

    private ProductVariant()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public long ProductId { get; private set; }
    public string Size { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public int Position { get; private set; }
    public bool IsActive { get; private set; }

    public string Label => string.Join(
        LabelSeparator,
        new[] { Size, Color is null ? string.Empty : ProductColors.NameOf(Color) }.Where(part => part.Length > 0));

    internal static ProductVariant Create(long productId, string size, string? color, int position) =>
        new()
        {
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
