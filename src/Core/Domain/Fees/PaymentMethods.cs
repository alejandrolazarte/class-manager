namespace ClassManager.Core.Domain.Fees;

public static class PaymentMethods
{
    public const string OnlineIsRecordedByStripeMessage = "Online payments are recorded when Stripe confirms them.";

    public static bool CanBeRecordedByHand(PaymentMethod method) => method != PaymentMethod.Online;
}
