using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Products;

public sealed class StockMovement : ITenantOwned
{
    public const int MaximumQuantity = 10_000;
    public const int NoteMaxLength = 200;

    private const string RestockQuantityMessage = "Load between 1 and 10,000 units.";
    private const string AdjustmentQuantityMessage = "The adjustment must be between -10,000 and 10,000 units, and not 0.";
    private const string AdjustmentNoteMessage = "Explain the adjustment in a note of at most 200 characters.";
    private const string NoteLengthMessage = "The note must be at most 200 characters.";
    private const string NegativeStockMessage = "The stock can't go below 0.";
    private const string StockNotTrackedMessage = "This product doesn't keep stock.";

    private StockMovement()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ProductVariantId { get; private set; }
    public StockMovementKind Kind { get; private set; }
    public int Quantity { get; private set; }
    public Guid? OrderId { get; private set; }
    public string? Note { get; private set; }
    public Guid? RecordedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Result<StockMovement> Load(
        Product product,
        ProductVariant variant,
        StockMovementKind? kind,
        int? quantity,
        string? note,
        int currentStock,
        Guid? recordedByUserId,
        DateTimeOffset createdAt)
    {
        if (!product.TracksStock)
        {
            return Result.Validation<StockMovement>(StockNotTrackedMessage, ProductErrorCodes.StockNotTracked);
        }

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote?.Length > NoteMaxLength)
        {
            return Result.Validation<StockMovement>(NoteLengthMessage, fieldName: nameof(Note));
        }

        switch (kind)
        {
            case StockMovementKind.Restock when quantity is null or < 1 or > MaximumQuantity:
                return Result.Validation<StockMovement>(RestockQuantityMessage, fieldName: nameof(Quantity));
            case StockMovementKind.Adjustment when quantity is null or 0 or < -MaximumQuantity or > MaximumQuantity:
                return Result.Validation<StockMovement>(AdjustmentQuantityMessage, fieldName: nameof(Quantity));
            case StockMovementKind.Adjustment when trimmedNote is null:
                return Result.Validation<StockMovement>(AdjustmentNoteMessage, fieldName: nameof(Note));
            case StockMovementKind.Adjustment when quantity < 0 && currentStock + quantity < 0:
                return Result.Validation<StockMovement>(NegativeStockMessage, fieldName: nameof(Quantity));
            case StockMovementKind.Restock or StockMovementKind.Adjustment:
                return Create(variant.Id, kind.Value, quantity!.Value, null, trimmedNote, recordedByUserId, createdAt);
            default:
                return Result.Validation<StockMovement>(RestockQuantityMessage, fieldName: nameof(Kind));
        }
    }

    public static StockMovement Sale(Guid variantId, int quantity, Guid orderId, Guid? recordedByUserId, DateTimeOffset createdAt) =>
        Create(variantId, StockMovementKind.Sale, -quantity, orderId, null, recordedByUserId, createdAt);

    public static StockMovement Return(Guid variantId, int quantity, Guid orderId, Guid? recordedByUserId, DateTimeOffset createdAt) =>
        Create(variantId, StockMovementKind.Refund, quantity, orderId, null, recordedByUserId, createdAt);

    private static StockMovement Create(
        Guid variantId,
        StockMovementKind kind,
        int quantity,
        Guid? orderId,
        string? note,
        Guid? recordedByUserId,
        DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.CreateVersion7(),
            ProductVariantId = variantId,
            Kind = kind,
            Quantity = quantity,
            OrderId = orderId,
            Note = note,
            RecordedByUserId = recordedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
