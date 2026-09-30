namespace ClassManager.Infrastructure.WebPush;

internal interface IWebPushSender
{
    Task<PushDelivery> SendAsync(PushTarget target, string payload, CancellationToken cancellationToken);
}
