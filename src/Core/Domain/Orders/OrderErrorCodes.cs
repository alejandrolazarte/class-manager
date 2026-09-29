namespace ClassManager.Core.Domain.Orders;

public static class OrderErrorCodes
{
    public const string NotFound = "order.not_found";
    public const string LineNotFound = "order.line_not_found";
    public const string NotAwaitingPickup = "order.not_awaiting_pickup";
    public const string NotRefundable = "order.not_refundable";
    public const string NothingToRefund = "order.nothing_to_refund";
    public const string ClientRequired = "order.client_required";
    public const string PurchaseFromOrder = "class_pack_purchase.from_order";
}
