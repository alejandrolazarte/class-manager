namespace ClassManager.Core.Abstractions.OnlinePayments;

public sealed record OnlineCheckoutCompleted(long BusinessId, long OrderId, decimal Amount, string ExternalPaymentId);
