using System.Globalization;
using System.Net.Http.Json;

namespace ClassManager.Backoffice.Api.I.Tests.Logs;

internal static class LogRequests
{
    private const string CommandLogRoute = "/api/logs/commands";
    private const string SecurityEventsRoute = "/api/logs/security-events";
    private const string DayFormat = "yyyy-MM-dd";
    private const int MaximumAttempts = 50;

    private static readonly TimeSpan AttemptDelay = TimeSpan.FromMilliseconds(100);

    public static async Task<CommandLogResponse> GetCommandLogAsync(this HttpClient client, string accessToken, DateOnly day, Guid? brandId = null)
    {
        var route = $"{CommandLogRoute}?day={day.ToString(DayFormat, CultureInfo.InvariantCulture)}"
            + (brandId is { } id ? $"&brandId={id}" : string.Empty);
        using var response = await client.GetAsync(route, accessToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<CommandLogResponse>())!;
    }

    public static async Task<CommandLogResponse> WaitForCommandLogAsync(
        this HttpClient client,
        string accessToken,
        DateOnly day,
        Func<CommandLogEntryResponse, bool> isExpected)
    {
        for (var attempt = 1; ; attempt++)
        {
            var log = await client.GetCommandLogAsync(accessToken, day);
            if (log.Entries.Any(isExpected) || attempt == MaximumAttempts)
            {
                return log;
            }

            await Task.Delay(AttemptDelay);
        }
    }

    public static async Task<SecurityEventListResponse> GetSecurityEventsAsync(this HttpClient client, string accessToken, DateOnly day)
    {
        using var response = await client.GetAsync(
            $"{SecurityEventsRoute}?day={day.ToString(DayFormat, CultureInfo.InvariantCulture)}",
            accessToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<SecurityEventListResponse>())!;
    }
}

internal sealed record CommandLogResponse(DateOnly Day, IReadOnlyList<CommandLogEntryResponse> Entries, bool IsTruncated);

internal sealed record CommandLogEntryResponse(string Command, string Outcome, string Source, Guid? BrandId, string? BrandName, string? Payload);

internal sealed record SecurityEventListResponse(DateOnly Day, IReadOnlyList<SecurityEventEntryResponse> Events);

internal sealed record SecurityEventEntryResponse(string Event, string? Route, string? IpAddress, string? BrandName)
{
    public IReadOnlyList<SecurityEventRouteResponse>? Routes { get; init; }

    public int? OtherRouteRequests { get; init; }
}

internal sealed record SecurityEventRouteResponse(string Method, string Route, int Count);
