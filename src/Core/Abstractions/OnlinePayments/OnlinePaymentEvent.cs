namespace ClassManager.Core.Abstractions.OnlinePayments;

public sealed record OnlinePaymentEvent(string Id, string? UpdatedExternalAccountId, OnlineCheckoutCompleted? CompletedCheckout = null);
