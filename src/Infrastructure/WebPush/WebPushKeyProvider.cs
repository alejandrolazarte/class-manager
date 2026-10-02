using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Notifications.WebPush;
using Microsoft.Extensions.Options;

namespace ClassManager.Infrastructure.WebPush;

internal sealed class WebPushKeyProvider(IOptions<VapidOptions> options) : IWebPushKeyProvider
{
    public string? PublicKey => options.Value.IsConfigured ? options.Value.PublicKey : null;
}
