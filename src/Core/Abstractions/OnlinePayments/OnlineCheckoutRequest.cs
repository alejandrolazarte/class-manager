namespace ClassManager.Core.Abstractions.OnlinePayments;

public sealed record OnlineCheckoutRequest(
    string ExternalAccountId,
    string Description,
    decimal Amount,
    string CurrencyCode,
    decimal PlatformFee,
    long BusinessId,
    long OrderId,
    Uri SuccessUrl,
    Uri CancelUrl);
