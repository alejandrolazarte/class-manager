using ClassManager.Subscriptions.Billing;
using ClassManager.Subscriptions.Stripe.Billing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;

namespace ClassManager.Subscriptions.Stripe.Hosting;

public static class StripeBillingServiceCollectionExtensions
{
    public static IServiceCollection AddStripeBilling(this IServiceCollection services)
    {
        services.AddOptions<StripeBillingOptions>().BindConfiguration(StripeBillingOptions.SectionName);
        services.AddSingleton<IBillingGateway>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<StripeBillingOptions>>().Value;
            return string.IsNullOrWhiteSpace(options.SecretKey)
                ? new UnconfiguredBillingGateway()
                : new StripeBillingGateway(new StripeClient(options.SecretKey));
        });

        return services;
    }
}
