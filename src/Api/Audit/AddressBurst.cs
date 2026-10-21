using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.Audit;

internal sealed record AddressBurst(string IpAddress, int Count, string? UserAgent, IReadOnlyList<SecurityEventRoute> Routes, int OtherRouteRequests);
