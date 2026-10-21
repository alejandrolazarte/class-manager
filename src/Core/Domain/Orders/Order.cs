using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Orders;

public sealed class Order : ITenantOwned, IAuditable
{
    public const int NotesMaxLength = 200;
    public const int MaximumLineCount = 20;
    public const int MaximumOpenRequestsPerClient = 5;

    public static readonly TimeSpan RequestLifetime = TimeSpan.FromDays(7);

    private const string LinesRequiredMessage = "Add at least one item, and at most 20.";
    private const string MethodRequiredMessage = "Choose a payment method.";
    private const string PaidOnInFutureMessage = "The sale date can't be in the future.";
    private const string NotesLengthMessage = "Notes must be at most 200 characters.";
    private const string ClientRequiredMessage = "Choose the client to credit the classes to.";
    private const string NotAwaitingPickupMessage = "This order has nothing waiting to be picked up.";
    private const string NotPendingMessage = "This order was already handed over or cancelled.";
    private const string NotRefundableMessage = "Only orders that were paid or handed over can be refunded.";
    private const string NothingOwedMessage = "This order has nothing left to pay.";
    private const string AmountOverBalanceMessage = "The amount is more than what is left to pay.";
    private const string PaymentNotFoundMessage = "The payment is not part of this order.";
    private const string PaymentLockedMessage = "The payments of a cancelled order were given back and can't change.";
    private const string NotCancellableMessage = "Only orders that were not handed over can be cancelled.";
    private const string PaidNotCancellableMessage = "Only orders without payments can be cancelled from the app.";
    private const string NothingToRefundMessage = "There are no unused classes left to refund.";
    private const string NothingToDeliverMessage = "This order has no products to deliver.";
    private const string DeliveryClassRequiredMessage = "Choose the class where the products are handed over.";
    private const string AlreadyReadyMessage = "This order is already marked ready.";
    private const int RefundDecimals = 2;
    private const string AlreadyNumberedMessage = "The order already has a number.";

    private readonly List<OrderLine> _lines = [];
    private readonly List<OrderPayment> _payments = [];

    private Order()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public int Number { get; private set; }
    public Guid? ClientId { get; private set; }
    public OrderChannel Channel { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public Guid? DeliveredByUserId { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public DeliveryMethod Delivery { get; private set; }
    public Guid? DeliveryClassGroupId { get; private set; }
    public DateTimeOffset? ReadyAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public Guid? CancelledByUserId { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines;
    public IReadOnlyList<OrderPayment> Payments => _payments;

    public decimal Total => _lines.Sum(line => line.Total);

    public decimal RefundedAmount => _lines.Sum(line => line.RefundedAmount);

    public decimal AmountDue => Total - RefundedAmount;

    public decimal PaidAmount => _payments.Sum(payment => payment.Amount);

    public decimal Balance => Math.Max(AmountDue - PaidAmount, 0);

    public decimal CollectedAmount => Math.Min(PaidAmount, AmountDue);

    public decimal ReturnedAmount => PaidAmount - CollectedAmount;

    public DateOnly? LastPaidOn => _payments.Count == 0 ? null : _payments.Max(payment => payment.PaidOn);

    public OrderPaymentStatus PaymentStatus => Status == OrderStatus.Cancelled || (PaidAmount == 0 && Balance > 0)
        ? OrderPaymentStatus.Unpaid
        : Balance > 0 ? OrderPaymentStatus.Partial : OrderPaymentStatus.Paid;

    public bool HasProducts => _lines.Any(line => line.Kind == OrderLineKind.Product);

    public bool AwaitsPickup => Status == OrderStatus.Pending && HasProducts;

    public bool IsReady => ReadyAt is not null;

    public static Result<Order> CounterSale(
        Guid? clientId,
        IReadOnlyList<OrderLine> lines,
        string currencyCode,
        decimal? paidAmount,
        PaymentMethod? method,
        DateOnly paidOn,
        string? notes,
        bool isDelivered,
        DateOnly today,
        Guid? recordedByUserId,
        DateTimeOffset createdAt)
    {
        var order = Create(clientId, OrderChannel.Counter, lines, currencyCode, notes, recordedByUserId, createdAt);
        if (order.IsFailure)
        {
            return order;
        }

        var amount = paidAmount ?? order.Value!.Total;
        if (amount != 0)
        {
            var payment = order.Value!.RecordPayment(amount, method, paidOn, today, recordedByUserId, createdAt);
            if (payment.IsFailure)
            {
                return payment.Error!;
            }
        }

        if (isDelivered)
        {
            order.Value!.MarkDelivered(recordedByUserId, createdAt);
        }

        return order;
    }

    public static Result<Order> Request(
        Guid clientId,
        IReadOnlyList<OrderLine> lines,
        string currencyCode,
        Guid? requestedByUserId,
        DateTimeOffset createdAt) =>
        Create(clientId, OrderChannel.App, lines, currencyCode, null, requestedByUserId, createdAt);

    public void AssignNumber(int number)
    {
        if (Number != 0)
        {
            throw new InvalidOperationException(AlreadyNumberedMessage);
        }

        Number = number;
    }

    public Result RecordPayment(
        decimal? amount, PaymentMethod? method, DateOnly paidOn, DateOnly today, Guid? recordedByUserId, DateTimeOffset recordedAt)
    {
        if (Status == OrderStatus.Cancelled || Balance == 0)
        {
            return Result.Conflict(NothingOwedMessage, OrderErrorCodes.NothingOwed);
        }

        var amountValidation = MonthlyFee.Validate(amount ?? 0, nameof(OrderPayment.Amount));
        if (amountValidation.IsFailure)
        {
            return amountValidation;
        }

        if (amount > Balance)
        {
            return Result.Validation(AmountOverBalanceMessage, OrderErrorCodes.AmountOverBalance, nameof(OrderPayment.Amount));
        }

        if (method is null || !Enum.IsDefined(method.Value))
        {
            return Result.Validation(MethodRequiredMessage, fieldName: nameof(OrderPayment.Method));
        }

        if (paidOn > today)
        {
            return Result.Validation(PaidOnInFutureMessage, fieldName: nameof(OrderPayment.PaidOn));
        }

        _payments.Add(OrderPayment.Create(Id, amount!.Value, method.Value, paidOn, recordedByUserId, recordedAt));
        return Result.Success();
    }

    public Result<OrderPayment> DeletePayment(Guid paymentId)
    {
        var payment = _payments.FirstOrDefault(candidate => candidate.Id == paymentId);
        if (payment is null)
        {
            return Result.NotFound<OrderPayment>(PaymentNotFoundMessage, OrderErrorCodes.PaymentNotFound);
        }

        if (Status == OrderStatus.Cancelled)
        {
            return Result.Conflict<OrderPayment>(PaymentLockedMessage, OrderErrorCodes.PaymentLocked);
        }

        _payments.Remove(payment);
        return payment;
    }

    public Result RestorePayment(OrderPayment payment)
    {
        if (payment.OrderId != Id || !payment.IsDeleted)
        {
            return Result.NotFound(PaymentNotFoundMessage, OrderErrorCodes.PaymentNotFound);
        }

        if (Status == OrderStatus.Cancelled)
        {
            return Result.Conflict(PaymentLockedMessage, OrderErrorCodes.PaymentLocked);
        }

        var paidByOthers = _payments.Where(candidate => candidate.Id != payment.Id).Sum(candidate => candidate.Amount);
        if (payment.Amount > AmountDue - paidByOthers)
        {
            return Result.Conflict(AmountOverBalanceMessage, OrderErrorCodes.AmountOverBalance);
        }

        payment.Restore();
        if (!_payments.Contains(payment))
        {
            _payments.Add(payment);
        }

        return Result.Success();
    }

    public Result CancelUnpaid(Guid? cancelledByUserId, DateTimeOffset cancelledAt) =>
        _payments.Count > 0
            ? Result.Conflict(PaidNotCancellableMessage, OrderErrorCodes.NotCancellable)
            : Cancel(cancelledByUserId, cancelledAt);

    public Result Cancel(Guid? cancelledByUserId, DateTimeOffset cancelledAt)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Conflict(NotCancellableMessage, OrderErrorCodes.NotCancellable);
        }

        if (_payments.Count > 0)
        {
            foreach (var line in _lines)
            {
                line.RefundRemaining();
            }
        }

        Status = OrderStatus.Cancelled;
        CancelledAt = cancelledAt.ToUniversalTime();
        CancelledByUserId = cancelledByUserId;
        return Result.Success();
    }

    public DateTimeOffset? ExpiresAt =>
        Channel == OrderChannel.App && Status == OrderStatus.Pending && _payments.Count == 0 ? CreatedAt + RequestLifetime : null;

    public bool IsExpired(DateTimeOffset now) => ExpiresAt <= now;

    private static Result<Order> Create(
        Guid? clientId,
        OrderChannel channel,
        IReadOnlyList<OrderLine> lines,
        string currencyCode,
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
            CurrencyCode = currencyCode,
            Status = OrderStatus.Pending,
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

    public Result ChooseDelivery(DeliveryMethod? delivery, Guid? classGroupId)
    {
        if (!AwaitsPickup)
        {
            return Result.Conflict(NothingToDeliverMessage, OrderErrorCodes.NotAwaitingPickup);
        }

        switch (delivery)
        {
            case DeliveryMethod.Pickup:
                Delivery = DeliveryMethod.Pickup;
                DeliveryClassGroupId = null;
                return Result.Success();
            case DeliveryMethod.InClass when classGroupId is not null && ClientId is not null:
                Delivery = DeliveryMethod.InClass;
                DeliveryClassGroupId = classGroupId;
                return Result.Success();
            default:
                return Result.Validation(DeliveryClassRequiredMessage, OrderErrorCodes.ClassNotValid, nameof(DeliveryClassGroupId));
        }
    }

    public Result MarkReady(DateTimeOffset readyAt)
    {
        if (!AwaitsPickup)
        {
            return Result.Conflict(NotAwaitingPickupMessage, OrderErrorCodes.NotAwaitingPickup);
        }

        if (IsReady)
        {
            return Result.Conflict(AlreadyReadyMessage, OrderErrorCodes.AlreadyReady);
        }

        ReadyAt = readyAt.ToUniversalTime();
        return Result.Success();
    }

    public Result MarkDelivered(Guid? deliveredByUserId, DateTimeOffset deliveredAt)
    {
        if (Status != OrderStatus.Pending)
        {
            return Result.Conflict(NotPendingMessage, OrderErrorCodes.NotPending);
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
        Status == OrderStatus.Delivered || (Status == OrderStatus.Pending && _payments.Count > 0)
            ? Result.Success()
            : Result.Conflict(NotRefundableMessage, OrderErrorCodes.NotRefundable);
}
