using ClassManager.Notifications.Delivery.Email;
using ClassManager.Notifications.Delivery.WebPush;
using ClassManager.Notifications.Email;
using ClassManager.Notifications.WebPush;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ClassManager.Notifications.Delivery.Hosting;

public static class NotificationsServiceCollectionExtensions
{
    private static readonly TimeSpan WebPushTimeout = TimeSpan.FromSeconds(10);

    public static IServiceCollection AddEmailTransport(this IServiceCollection services)
    {
        services.AddOptions<SmtpOptions>().BindConfiguration(SmtpOptions.SectionName);
        services.AddSingleton<SmtpEmailTransport>();
        services.AddSingleton<LoggingEmailTransport>();
        services.AddSingleton<IEmailTransport>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SmtpOptions>>().Value.IsConfigured
                ? serviceProvider.GetRequiredService<SmtpEmailTransport>()
                : serviceProvider.GetRequiredService<LoggingEmailTransport>());

        return services;
    }

    public static IServiceCollection AddWebPushSender(this IServiceCollection services)
    {
        services.AddOptions<VapidOptions>().BindConfiguration(VapidOptions.SectionName);
        services.AddHttpClient<IWebPushSender, WebPushSender>(client => client.Timeout = WebPushTimeout);

        return services;
    }
}
