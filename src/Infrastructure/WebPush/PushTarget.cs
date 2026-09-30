namespace ClassManager.Infrastructure.WebPush;

internal sealed record PushTarget(string Endpoint, string P256dh, string Auth);
