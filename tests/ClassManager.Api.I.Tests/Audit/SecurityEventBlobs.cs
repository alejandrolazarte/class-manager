using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal static class SecurityEventBlobs
{
    private static readonly TimeSpan WaitLimit = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(200);

    public static BlobContainerClient Container(ApiFixture fixture) =>
        new BlobServiceClient(fixture.StorageConnectionString).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName);

    public static async Task<IReadOnlyList<JsonElement>> ReadDayAsync(BlobContainerClient container, DateTimeOffset day)
    {
        try
        {
            var content = await container.GetBlobClient(AppendBlobSecurityEventStore.BlobNameOf(day)).DownloadContentAsync();
            return [.. Encoding.UTF8.GetString(content.Value.Content.ToArray())
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => JsonDocument.Parse(line).RootElement)];
        }
        catch (RequestFailedException)
        {
            return [];
        }
    }

    public static async Task<IReadOnlyList<JsonElement>> WaitForAsync(ApiFixture fixture, Func<JsonElement, bool> isExpected)
    {
        var container = Container(fixture);
        var waitUntil = DateTime.UtcNow + WaitLimit;
        while (DateTime.UtcNow < waitUntil)
        {
            var lines = await ReadDayAsync(container, BusinessApiFactory.Now);
            if (lines.Any(isExpected))
            {
                return lines;
            }

            await Task.Delay(PollInterval);
        }

        throw new TimeoutException("The security event was not written.");
    }

    public static string? Text(JsonElement line, string propertyName) =>
        line.TryGetProperty(propertyName, out var value) ? value.GetString() : null;
}
