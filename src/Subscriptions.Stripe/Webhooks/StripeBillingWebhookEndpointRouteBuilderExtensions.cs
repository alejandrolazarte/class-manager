using ClassManager.Subscriptions.AspNetCore.Billing;
using ClassManager.Subscriptions.AspNetCore.Endpoints;
using ClassManager.Subscriptions.Stripe.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ClassManager.Subscriptions.Stripe.Webhooks;

public static class StripeBillingWebhookEndpointRouteBuilderExtensions
{
    public const string SignatureHeaderName = "Stripe-Signature";

    public static RouteHandlerBuilder MapStripeBillingWebhook(this IEndpointRouteBuilder endpoints, string pattern) =>
        endpoints.MapPost(pattern, ReceiveAsync)
            .AllowAnonymous()
            .AllowInactiveSubscription()
            .ExcludeFromDescription();

    private static async Task<IResult> ReceiveAsync(
        HttpRequest request,
        IOptions<StripeBillingOptions> options,
        IBillingWebhookProcessor processor,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(request.Body);
        var payload = await reader.ReadToEndAsync(cancellationToken);
        var signatureHeader = request.Headers[SignatureHeaderName].ToString();

        var billingEvent = StripeWebhookEvents.Read(payload, signatureHeader, options.Value.WebhookSecret);
        if (billingEvent is null)
        {
            return TypedResults.BadRequest();
        }

        await processor.ProcessAsync(billingEvent, cancellationToken);
        return TypedResults.Ok();
    }
}
