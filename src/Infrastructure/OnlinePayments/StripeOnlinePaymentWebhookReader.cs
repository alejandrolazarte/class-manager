using ClassManager.Core.Abstractions.OnlinePayments;
using Microsoft.Extensions.Options;
using Stripe;

namespace ClassManager.Infrastructure.OnlinePayments;

internal sealed class StripeOnlinePaymentWebhookReader(IOptions<OnlinePaymentOptions> options) : IOnlinePaymentWebhookReader
{
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
        return new OnlinePaymentEvent(stripeEvent.Id, updatedAccountId);
    }
}
