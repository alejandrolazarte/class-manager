namespace ClassManager.Core.Domain.Orders;

public static class OrderErrorCodes
{
    public const string NotFound = "order.not_found";
    public const string LineNotFound = "order.line_not_found";
    public const string NotAwaitingPickup = "order.not_awaiting_pickup";
    public const string NotPending = "order.not_pending";
    public const string NotRefundable = "order.not_refundable";
    public const string NothingToRefund = "order.nothing_to_refund";
    public const string ClientRequired = "order.client_required";
    public const string NotCancellable = "order.not_cancellable";
    public const string NothingOwed = "order.nothing_owed";
    public const string AmountOverBalance = "order.amount_over_balance";
    public const string TooManyOpen = "order.too_many_open";
    public const string ClassNotValid = "order.class_not_valid";
    public const string AlreadyReady = "order.already_ready";
}
