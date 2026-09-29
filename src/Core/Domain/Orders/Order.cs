using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Orders;

public sealed class Order : ITenantOwned
{
    public const int NotesMaxLength = 200;
    public const int MaximumLineCount = 20;
    public const int MaximumOpenRequestsPerFamily = 5;

    public static readonly TimeSpan RequestLifetime = TimeSpan.FromDays(7);

    private const string LinesRequiredMessage = "Add at least one item, and at most 20.";
    private const string MethodRequiredMessage = "Choose a payment method.";
    private const string PaidOnInFutureMessage = "The sale date can't be in the future.";
    private const string NotesLengthMessage = "Notes must be at most 200 characters.";
    private const string ClientRequiredMessage = "Choose the family to credit the classes to.";
    private const string NotAwaitingPickupMessage = "This order has nothing waiting to be picked up.";
    private const string NotRefundableMessage = "Only paid orders can be refunded.";
    private const string NothingToRefundMessage = "There are no unused classes left to refund.";
    private const string NotRequestedMessage = "Only orders waiting for payment can be paid or cancelled.";
    private const int RefundDecimals = 2;

    private readonly List<OrderLine> _lines = [];

    private Order()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid? ClientId { get; private set; }
    public OrderChannel Channel { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentMethod? Method { get; private set; }
    public DateOnly? PaidOn { get; private set; }
    public Guid? PaymentRecordedByUserId { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public Guid? DeliveredByUserId { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public Guid? CancelledByUserId { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines;

    public decimal Total => _lines.Sum(line => line.Total);

    public decimal RefundedAmount => _lines.Sum(line => line.RefundedAmount);

    public bool HasProducts => _lines.Any(line => line.Kind == OrderLineKind.Product);

    public bool AwaitsPickup => Status == OrderStatus.Paid && HasProducts;

    public static Result<Order> CounterSale(
        Guid? clientId,
        IReadOnlyList<OrderLine> lines,
        PaymentMethod? method,
        DateOnly paidOn,
        string? notes,
        bool isDelivered,
        DateOnly today,
        Guid? recordedByUserId,
        DateTimeOffset createdAt)
    {
        var order = Create(clientId, OrderChannel.Counter, lines, notes, recordedByUserId, createdAt);
        if (order.IsFailure)
        {
            return order;
        }

        var payment = order.Value!.ConfirmPayment(method, paidOn, today, recordedByUserId);
        if (payment.IsFailure)
        {
            return payment.Error!;
        }

        if (isDelivered && order.Value.HasProducts)
        {
            order.Value.MarkDelivered(recordedByUserId, createdAt);
        }

        return order;
    }

    public static Result<Order> Request(Guid clientId, IReadOnlyList<OrderLine> lines, Guid? requestedByUserId, DateTimeOffset createdAt) =>
        Create(clientId, OrderChannel.App, lines, null, requestedByUserId, createdAt);

    public Result ConfirmPayment(PaymentMethod? method, DateOnly paidOn, DateOnly today, Guid? recordedByUserId)
    {
        if (Status != OrderStatus.Requested)
        {
            return Result.Conflict(NotRequestedMessage, OrderErrorCodes.NotRequested);
        }

        if (method is null || !Enum.IsDefined(method.Value))
        {
            return Result.Validation(MethodRequiredMessage, fieldName: nameof(Method));
        }

        if (paidOn > today)
        {
            return Result.Validation(PaidOnInFutureMessage, fieldName: nameof(PaidOn));
        }

        Status = OrderStatus.Paid;
        Method = method;
        PaidOn = paidOn;
        PaymentRecordedByUserId = recordedByUserId;
        return Result.Success();
    }

    public Result Cancel(Guid? cancelledByUserId, DateTimeOffset cancelledAt)
    {
        if (Status != OrderStatus.Requested)
        {
            return Result.Conflict(NotRequestedMessage, OrderErrorCodes.NotRequested);
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = cancelledAt.ToUniversalTime();
        CancelledByUserId = cancelledByUserId;
        return Result.Success();
    }

    public bool IsExpired(DateTimeOffset now) => Status == OrderStatus.Requested && CreatedAt + RequestLifetime <= now;

    private static Result<Order> Create(
        Guid? clientId,
        OrderChannel channel,
        IReadOnlyList<OrderLine> lines,
        string? notes,
        Guid? createdByUserId,
        DateTimeOffset createdAt)
    {
        if (lines.Count is 0 or > MaximumLineCount)
        {
            return Result.Validation<Order>(LinesRequiredMessage, fieldName: OrderLine.LinesFieldName);
        }

        if (clientId is null && lines.Any(line => line.Kind == OrderLineKind.ClassPack))
        {
            return Result.Validation<Order>(ClientRequiredMessage, OrderErrorCodes.ClientRequired, nameof(ClientId));
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<Order>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        var order = new Order
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            Channel = channel,
            Status = OrderStatus.Requested,
            Notes = trimmedNotes,
            CreatedByUserId = createdByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
        foreach (var line in lines)
        {
            line.AttachTo(order.Id);
            order._lines.Add(line);
        }

        return order;
    }

    public OrderLine? FindLine(Guid lineId) => _lines.FirstOrDefault(line => line.Id == lineId);

    public Result MarkDelivered(Guid? deliveredByUserId, DateTimeOffset deliveredAt)
    {
        if (!AwaitsPickup)
        {
            return Result.Conflict(NotAwaitingPickupMessage, OrderErrorCodes.NotAwaitingPickup);
        }

        Status = OrderStatus.Delivered;
        DeliveredAt = deliveredAt.ToUniversalTime();
        DeliveredByUserId = deliveredByUserId;
        return Result.Success();
    }

    public Result RefundProductUnits(OrderLine line, int? quantity)
    {
        var refundable = EnsureRefundable();
        return refundable.IsFailure ? refundable : line.RefundUnits(quantity);
    }

    public Result<decimal> RefundUnusedClasses(OrderLine line, int classCount, int unusedClasses)
    {
        var refundable = EnsureRefundable();
        if (refundable.IsFailure)
        {
            return refundable.Error!;
        }

        if (line.IsFullyRefunded || unusedClasses <= 0 || classCount <= 0)
        {
            return Result.Validation<decimal>(NothingToRefundMessage, OrderErrorCodes.NothingToRefund, OrderLine.LinesFieldName);
        }

        var amount = decimal.Round(line.UnitPrice * unusedClasses / classCount, RefundDecimals);
        line.RefundClasses(amount);
        return amount;
    }

    private Result EnsureRefundable() =>
        Status is OrderStatus.Paid or OrderStatus.Delivered
            ? Result.Success()
            : Result.Conflict(NotRefundableMessage, OrderErrorCodes.NotRefundable);
}
