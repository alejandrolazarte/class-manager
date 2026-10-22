using System.Collections.Concurrent;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.Audit;

internal sealed class AddressActivity
{
    private readonly ConcurrentDictionary<RequestRoute, int> _requestsByRoute = new();
    private int _recorded;
    private int _unauthorized;
    private int _suppressed;
    private int _otherRouteRequests;
    private string? _userAgent;

    public int Unauthorized => _unauthorized;

    public int Suppressed => _suppressed;

    public int OtherRouteRequests => _otherRouteRequests;

    public string? UserAgent => _userAgent;

    public IReadOnlyList<SecurityEventRoute> Routes =>
        [.. _requestsByRoute
            .Select(route => new SecurityEventRoute(route.Key.Method, route.Key.Route, route.Value))
            .OrderByDescending(route => route.Count)
            .ThenBy(route => route.Route, StringComparer.Ordinal)
            .ThenBy(route => route.Method, StringComparer.Ordinal)];

    public int RecordOne() => Interlocked.Increment(ref _recorded);

    public void CountUnauthorized(RequestRoute? route, string userAgent)
    {
        Interlocked.Increment(ref _unauthorized);
        CountRequest(route, userAgent);
    }

    public void CountSuppressed(RequestRoute? route, string userAgent)
    {
        Interlocked.Increment(ref _suppressed);
        CountRequest(route, userAgent);
    }

    private void CountRequest(RequestRoute? route, string userAgent)
    {
        if (userAgent.Length > 0)
        {
            Volatile.Write(ref _userAgent, userAgent);
        }

        if (route is { } knownRoute
            && (_requestsByRoute.ContainsKey(knownRoute) || _requestsByRoute.Count < SecurityEventThrottle.MaximumTrackedRoutesPerAddress))
        {
            _requestsByRoute.AddOrUpdate(knownRoute, 1, (_, requests) => requests + 1);
            return;
        }

        Interlocked.Increment(ref _otherRouteRequests);
    }
}
