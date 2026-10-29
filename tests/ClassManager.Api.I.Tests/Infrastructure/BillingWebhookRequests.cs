using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Stripe.Webhooks;
using ClassManager.Subscriptions.Subscribers;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class BillingWebhookRequests
{
    public const string ProPlanCode = "pro";
    public const decimal ProPrice = 39m;
    public const string Euro = "EUR";
    public const string CheckoutSessionCompleted = "checkout.session.completed";
    public const string SubscriptionUpdated = "customer.subscription.updated";
    public const string SubscriptionDeleted = "customer.subscription.deleted";
    public const string InvoicePaymentFailed = "invoice.payment_failed";

    private const string EventIdPrefix = "evt_";
    private const string ExternalSubscriptionIdPrefix = "sub_";
    private const string JsonMediaType = "application/json";

    public static string NewEventId() => EventIdPrefix + Guid.NewGuid().ToString("N");

    public static string NewExternalSubscriptionId() => ExternalSubscriptionIdPrefix + Guid.NewGuid().ToString("N");

    public static ExternalSubscription ExternalPro(
        string externalSubscriptionId,
        long subscriberId,
        ExternalSubscriptionStatus status = ExternalSubscriptionStatus.Active,
        DateOnly? endedOn = null) =>
        new(externalSubscriptionId, subscriberId, PlanPriceLookupKey.For(ProPlanCode, Euro), ProPrice, Euro, status, CancelOn: null, endedOn);

    public static async Task<HttpResponseMessage> PostStripeEventAsync(
        this HttpClient httpClient,
        string eventType,
        string externalSubscriptionId,
        string? eventId = null,
        string webhookSecret = BusinessApiFactory.TestStripeWebhookSecret)
    {
        var payload = EventPayload(eventId ?? NewEventId(), eventType, externalSubscriptionId);
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(ApiRoutes.StripeBillingWebhook, UriKind.Relative))
        {
            Content = new StringContent(payload, Encoding.UTF8, JsonMediaType),
        };
        request.Headers.Add(StripeBillingWebhookEndpointRouteBuilderExtensions.SignatureHeaderName, Signature(payload, webhookSecret));
        return await httpClient.SendAsync(request);
    }

    public static async Task<Subscription> CurrentSubscriptionAsync(this ApiFixture fixture, SeededBusiness business)
    {
        await using var context = fixture.CreateDbContext(business.Business.Id);
        return await context.Subscriptions.AsNoTracking()
            .SingleAsync(subscription => subscription.SubscriberId == business.Business.OrganizationId && !subscription.IsDeleted);
    }

    public static async Task StartPayingThroughStripeAsync(this ApiFixture fixture, SeededBusiness business, string externalSubscriptionId, int daysAgo)
    {
        await using var context = fixture.CreateDbContext(business.Business.Id);
        var current = await context.Subscriptions
            .SingleAsync(subscription => subscription.SubscriberId == business.Business.OrganizationId && !subscription.IsDeleted);
        current.Delete(BusinessApiFactory.Now);
        context.Subscriptions.Add(Subscription.StartExternal(
            business.Business.OrganizationId,
            ProPlanCode,
            ProPrice,
            Euro,
            externalSubscriptionId,
            BusinessApiFactory.Now.AddDays(-daysAgo)));
        await context.SaveChangesAsync();
    }

    private static string EventPayload(string eventId, string eventType, string externalSubscriptionId) =>
        JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["id"] = eventId,
            ["object"] = "event",
            ["type"] = eventType,
            ["livemode"] = false,
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["data"] = new Dictionary<string, object> { ["object"] = EventObject(eventType, externalSubscriptionId) },
        });

    private static Dictionary<string, object> EventObject(string eventType, string externalSubscriptionId) =>
        eventType switch
        {
            CheckoutSessionCompleted => new()
            {
                ["id"] = "cs_" + Guid.NewGuid().ToString("N"),
                ["object"] = "checkout.session",
                ["mode"] = "subscription",
                ["subscription"] = externalSubscriptionId,
            },
            InvoicePaymentFailed => new()
            {
                ["id"] = "in_" + Guid.NewGuid().ToString("N"),
                ["object"] = "invoice",
                ["parent"] = new Dictionary<string, object>
                {
                    ["type"] = "subscription_details",
                    ["subscription_details"] = new Dictionary<string, object> { ["subscription"] = externalSubscriptionId },
                },
            },
            _ => new()
            {
                ["id"] = externalSubscriptionId,
                ["object"] = "subscription",
            },
        };

    private static string Signature(string payload, string webhookSecret)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(webhookSecret), Encoding.UTF8.GetBytes($"{timestamp}.{payload}"));
        return $"t={timestamp},v1={Convert.ToHexStringLower(hash)}";
    }
}
