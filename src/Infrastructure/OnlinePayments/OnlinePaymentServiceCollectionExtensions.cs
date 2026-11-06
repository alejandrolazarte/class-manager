using ClassManager.Core.Abstractions.OnlinePayments;
using ClassManager.Infrastructure.BackgroundTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;

namespace ClassManager.Infrastructure.OnlinePayments;

public static class OnlinePaymentServiceCollectionExtensions
{
    public static IServiceCollection AddOnlineOrderPaymentTasks(this IServiceCollection services) =>
        services.AddBackgroundTask<RecordOnlineOrderPaymentBackgroundTaskCommand, RecordOnlineOrderPaymentBackgroundTaskHandler>();

    internal static IServiceCollection AddOnlinePayments(this IServiceCollection services)
    {
        services.AddOptions<OnlinePaymentOptions>().BindConfiguration(OnlinePaymentOptions.SectionName);
        services.AddSingleton<IOnlinePaymentWebhookReader, StripeOnlinePaymentWebhookReader>();
        services.AddScoped<IOnlineOrderPaymentInbox, QueuedOnlineOrderPaymentInbox>();
        services.AddSingleton<IOnlinePaymentGateway>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OnlinePaymentOptions>>().Value;
            return string.IsNullOrWhiteSpace(options.SecretKey)
                ? new UnconfiguredOnlinePaymentGateway()
                : new StripeOnlinePaymentGateway(new StripeClient(options.SecretKey));
        });

        return services;
    }
}
