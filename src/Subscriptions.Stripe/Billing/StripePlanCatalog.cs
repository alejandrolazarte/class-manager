using ClassManager.Subscriptions.Billing;
using Stripe;
using Stripe.BillingPortal;

namespace ClassManager.Subscriptions.Stripe.Billing;

internal sealed class StripePlanCatalog(StripeClient client)
{
    public const string ProductIdPrefix = "plan-";
    public const string MonthlyInterval = "month";
    public const string TaxExclusive = "exclusive";
    public const string SoftwareAsAServiceTaxCode = "txcd_10103001";
    public const string PortalConfigurationOwner = "subscriptions";

    private const string ResourceMissingCode = "resource_missing";
    private const string RecurringPriceType = "recurring";
    private const string SwitchPrice = "price";
    private const string ProrateChanges = "create_prorations";
    private const string CancelAtPeriodEnd = "at_period_end";
    private const int SingleInterval = 1;
    private const int LargestPage = 100;
    private const decimal MinorUnitsPerMajorUnit = 100m;

    public static string ProductIdOf(string planCode) => ProductIdPrefix + planCode;

    public async Task<string> EnsurePriceAsync(BillingPlanPrice plan, CancellationToken cancellationToken)
    {
        var productId = await EnsureProductAsync(plan, cancellationToken);
        var currency = plan.Currency.ToLowerInvariant();
        var unitAmount = (long)decimal.Round(plan.UnitAmount * MinorUnitsPerMajorUnit);
        var prices = await client.V1.Prices.ListAsync(
            new PriceListOptions { Product = productId, Active = true, Currency = currency, Type = RecurringPriceType, Limit = LargestPage },
            cancellationToken: cancellationToken);
        var existingPrice = prices.Data.FirstOrDefault(price =>
            price.UnitAmount == unitAmount
            && price.TaxBehavior == TaxExclusive
            && price.Recurring is { Interval: MonthlyInterval, IntervalCount: SingleInterval });
        if (existingPrice is not null)
        {
            return existingPrice.Id;
        }

        var createdPrice = await client.V1.Prices.CreateAsync(
            new PriceCreateOptions
            {
                Product = productId,
                Currency = currency,
                UnitAmount = unitAmount,
                TaxBehavior = TaxExclusive,
                Recurring = new PriceRecurringOptions { Interval = MonthlyInterval, IntervalCount = SingleInterval },
                Metadata = PlanMetadata(plan.PlanCode),
            },
            cancellationToken: cancellationToken);
        return createdPrice.Id;
    }

    public async Task<string> EnsurePortalConfigurationAsync(IReadOnlyList<BillingPlanPrice> switchablePlans, CancellationToken cancellationToken)
    {
        var switchableProducts = new List<ConfigurationFeaturesSubscriptionUpdateProductOptions>();
        foreach (var plan in switchablePlans)
        {
            var priceId = await EnsurePriceAsync(plan, cancellationToken);
            switchableProducts.Add(new ConfigurationFeaturesSubscriptionUpdateProductOptions
            {
                Product = ProductIdOf(plan.PlanCode),
                Prices = [priceId],
            });
        }

        var features = new ConfigurationFeaturesOptions
        {
            InvoiceHistory = new ConfigurationFeaturesInvoiceHistoryOptions { Enabled = true },
            PaymentMethodUpdate = new ConfigurationFeaturesPaymentMethodUpdateOptions { Enabled = true },
            SubscriptionCancel = new ConfigurationFeaturesSubscriptionCancelOptions { Enabled = true, Mode = CancelAtPeriodEnd },
            SubscriptionUpdate = new ConfigurationFeaturesSubscriptionUpdateOptions
            {
                Enabled = switchableProducts.Count > 0,
                DefaultAllowedUpdates = [SwitchPrice],
                ProrationBehavior = ProrateChanges,
                Products = switchableProducts,
            },
        };
        var configurations = await client.V1.BillingPortal.Configurations.ListAsync(
            new ConfigurationListOptions { Active = true, Limit = LargestPage },
            cancellationToken: cancellationToken);
        var ownConfiguration = configurations.Data.FirstOrDefault(configuration =>
            configuration.Metadata.TryGetValue(StripeMetadataKeys.ManagedBy, out var owner) && owner == PortalConfigurationOwner);
        if (ownConfiguration is not null)
        {
            var updated = await client.V1.BillingPortal.Configurations.UpdateAsync(
                ownConfiguration.Id,
                new ConfigurationUpdateOptions { Features = features },
                cancellationToken: cancellationToken);
            return updated.Id;
        }

        var created = await client.V1.BillingPortal.Configurations.CreateAsync(
            new ConfigurationCreateOptions
            {
                Features = features,
                Metadata = new Dictionary<string, string>(StringComparer.Ordinal) { [StripeMetadataKeys.ManagedBy] = PortalConfigurationOwner },
            },
            cancellationToken: cancellationToken);
        return created.Id;
    }

    private async Task<string> EnsureProductAsync(BillingPlanPrice plan, CancellationToken cancellationToken)
    {
        var productId = ProductIdOf(plan.PlanCode);
        try
        {
            var product = await client.V1.Products.GetAsync(productId, cancellationToken: cancellationToken);
            if (!product.Metadata.TryGetValue(StripeMetadataKeys.PlanCode, out var planCode) || planCode != plan.PlanCode)
            {
                await client.V1.Products.UpdateAsync(
                    productId,
                    new ProductUpdateOptions { Metadata = PlanMetadata(plan.PlanCode) },
                    cancellationToken: cancellationToken);
            }

            return productId;
        }
        catch (StripeException exception) when (exception.StripeError?.Code == ResourceMissingCode)
        {
            await client.V1.Products.CreateAsync(
                new ProductCreateOptions
                {
                    Id = productId,
                    Name = plan.ProductName,
                    TaxCode = SoftwareAsAServiceTaxCode,
                    Metadata = PlanMetadata(plan.PlanCode),
                },
                cancellationToken: cancellationToken);
            return productId;
        }
    }

    private static Dictionary<string, string> PlanMetadata(string planCode) =>
        new(StringComparer.Ordinal) { [StripeMetadataKeys.PlanCode] = planCode };
}
