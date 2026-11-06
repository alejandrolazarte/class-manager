namespace ClassManager.Core.UseCases.OnlinePayments;

public static class OnlinePaymentErrorCodes
{
    public const string NotAvailable = "onlinePayments.not_available";
    public const string NotAvailableMessage = "Online payments aren't available in the branch's country yet.";
    public const string AccountNotFound = "onlinePayments.account_not_found";
    public const string AccountNotFoundMessage = "The online payment account was not found in this brand.";
    public const string NotConnected = "onlinePayments.not_connected";
    public const string NotConnectedMessage = "The branch has no online payment account.";
    public const string NotCharging = "onlinePayments.not_charging";
    public const string NotChargingMessage = "The branch can't charge online.";
    public const string NothingToPayOnline = "onlinePayments.nothing_to_pay";
    public const string NothingToPayOnlineMessage = "This order has nothing to pay online.";
    public const string OrderDescriptionPrefix = "Pedido #";
}
