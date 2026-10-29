using ClassManager.Subscriptions.Billing;
using Stripe;
using Stripe.Checkout;

namespace ClassManager.Subscriptions.Stripe.Webhooks;

internal static class StripeWebhookEvents
{
    public static BillingEvent? Read(string payload, string signatureHeader, string webhookSecret)
    {
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, webhookSecret, throwOnApiVersionMismatch: false);
        }
        catch (StripeException)
        {
            return null;
        }

        return new BillingEvent(stripeEvent.Id, SubscriptionIdOf(stripeEvent));
    }

    private static string? SubscriptionIdOf(Event stripeEvent) =>
        stripeEvent.Data.Object switch
        {
            Session session when stripeEvent.Type == EventTypes.CheckoutSessionCompleted => session.SubscriptionId,
            Subscription subscription => subscription.Id,
            Invoice invoice when stripeEvent.Type is EventTypes.InvoicePaid or EventTypes.InvoicePaymentFailed =>
                invoice.Parent?.SubscriptionDetails?.SubscriptionId,
            _ => null,
        };
}
