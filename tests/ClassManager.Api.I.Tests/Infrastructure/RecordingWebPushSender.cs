using System.Collections.Concurrent;
using System.Text.Json;

namespace ClassManager.Api.I.Tests.Infrastructure;

internal sealed class RecordingWebPushSender : IWebPushSender
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);
    private static readonly JsonSerializerOptions PayloadOptions = new(JsonSerializerDefaults.Web);

    private readonly ConcurrentQueue<(string Endpoint, string Payload)> _sent = new();
    private readonly ConcurrentDictionary<string, bool> _expiredEndpoints = new(StringComparer.Ordinal);

    public Task<PushDelivery> SendAsync(PushTarget target, string payload, CancellationToken cancellationToken)
    {
        _sent.Enqueue((target.Endpoint, payload));
        return Task.FromResult(_expiredEndpoints.ContainsKey(target.Endpoint) ? PushDelivery.Gone : PushDelivery.Delivered);
    }

    public void Expire(string endpoint) => _expiredEndpoints[endpoint] = true;

    public async Task<PushMessage> WaitForPushToAsync(string endpoint)
    {
        var deadline = DateTimeOffset.UtcNow + WaitTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            var sent = _sent.Where(push => push.Endpoint == endpoint).Select(push => push.Payload).LastOrDefault();
            if (sent is not null)
            {
                return JsonSerializer.Deserialize<PushMessage>(sent, PayloadOptions)!;
            }

            await Task.Delay(PollInterval);
        }

        throw new TimeoutException($"No push reached {endpoint}.");
    }
}
