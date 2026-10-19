using System.Collections.Concurrent;
using ClassManager.Infrastructure.Audit;
using Microsoft.Extensions.Options;

namespace ClassManager.Api.Audit;

internal sealed class SecurityEventThrottle(IOptions<AuditLogOptions> options)
{
    public const int UnauthorizedBurstThreshold = 20;
    public const int MaximumTrackedAddresses = 10_000;

    private ConcurrentDictionary<string, AddressActivity> _activityByAddress = new();

    public bool TryRecord(string ipAddress) =>
        ActivityOf(ipAddress) is { } activity
        && activity.RecordOne() <= options.Value.MaximumSecurityEventsPerAddressPerMinute;

    public void CountUnauthorized(string ipAddress) => ActivityOf(ipAddress)?.CountUnauthorized();

    public void CountSuppressed(string ipAddress) => ActivityOf(ipAddress)?.CountSuppressed();

    public IReadOnlyList<(string IpAddress, int Count)> TakeBursts()
    {
        var finishedMinute = Interlocked.Exchange(ref _activityByAddress, new ConcurrentDictionary<string, AddressActivity>());
        return [.. finishedMinute
            .Where(address => address.Value.Unauthorized >= UnauthorizedBurstThreshold || address.Value.Suppressed > 0)
            .Select(address => (address.Key, address.Value.Unauthorized + address.Value.Suppressed))];
    }

    private AddressActivity? ActivityOf(string ipAddress)
    {
        var activityByAddress = _activityByAddress;
        if (activityByAddress.TryGetValue(ipAddress, out var activity))
        {
            return activity;
        }

        return activityByAddress.Count < MaximumTrackedAddresses ? activityByAddress.GetOrAdd(ipAddress, _ => new AddressActivity()) : null;
    }
}
