using System.Globalization;
using ClassManager.Core.Abstractions.OnlinePayments;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ClassManager.Infrastructure.OnlinePayments;

internal sealed class StripeOnlinePaymentWebhookReader(IOptions<OnlinePaymentOptions> options) : IOnlinePaymentWebhookReader
{
    private const string PaidStatus = "paid";

    public OnlinePaymentEvent? Read(string payload, string signatureHeader)
    {
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                payload,
                signatureHeader,
                options.Value.ConnectWebhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException)
        {
            return null;
        }

        var updatedAccountId = stripeEvent.Type == EventTypes.AccountUpdated
            ? (stripeEvent.Data.Object as Account)?.Id ?? stripeEvent.Account
            : null;
        var completedCheckout = stripeEvent.Type == EventTypes.CheckoutSessionCompleted && stripeEvent.Data.Object is Session session
            ? CompletedCheckoutOf(session)
            : null;
        return new OnlinePaymentEvent(stripeEvent.Id, updatedAccountId, completedCheckout);
    }

    private static OnlineCheckoutCompleted? CompletedCheckoutOf(Session session)
    {
        if (session.PaymentStatus != PaidStatus
            || !TryReadId(session.Metadata, StripeOnlinePaymentGateway.BusinessIdMetadataKey, out var businessId)
            || !TryReadId(session.Metadata, StripeOnlinePaymentGateway.OrderIdMetadataKey, out var orderId)
            || session.AmountTotal is not { } amountTotal)
        {
            return null;
        }

        return new OnlineCheckoutCompleted(
            businessId,
            orderId,
            amountTotal / StripeOnlinePaymentGateway.MinorUnitsPerMajorUnit,
            session.PaymentIntentId ?? session.Id);
    }

    private static bool TryReadId(Dictionary<string, string>? metadata, string key, out long id)
    {
        id = 0;
        return metadata is not null
            && metadata.TryGetValue(key, out var text)
            && long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out id);
    }
}
