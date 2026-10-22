using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace ClassManager.Infrastructure.Audit;

public sealed class AppendBlobSecurityEventStore(BlobContainerClient container, bool createsContainer) : IAuditLogStore<SecurityEvent>
{
    public const string ContainerName = "security-events";

    private const string DayBlobNameFormat = "yyyy/MM/dd";
    private const string DayBlobExtension = ".jsonl";
    private const char LineSeparator = '\n';

    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private bool _containerExists = !createsContainer;

    public static string BlobNameOf(DateTimeOffset occurredAt) =>
        occurredAt.UtcDateTime.ToString(DayBlobNameFormat, CultureInfo.InvariantCulture) + DayBlobExtension;

    public static IReadOnlyList<SecurityEvent> ReadLines(string lines) =>
        [.. lines.Split(LineSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => JsonSerializer.Deserialize<SecurityEvent>(line, Options))
            .OfType<SecurityEvent>()];

    public async Task WriteAsync(IReadOnlyList<SecurityEvent> entries, CancellationToken cancellationToken)
    {
        if (!_containerExists)
        {
            await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
            _containerExists = true;
        }

        foreach (var day in entries.GroupBy(entry => BlobNameOf(entry.OccurredAt)))
        {
            await AppendAsync(container.GetAppendBlobClient(day.Key), LinesOf(day), cancellationToken);
        }
    }

    private static async Task AppendAsync(AppendBlobClient blob, byte[] lines, CancellationToken cancellationToken)
    {
        try
        {
            await AppendBlockAsync(blob, lines, cancellationToken);
        }
        catch (RequestFailedException exception) when (exception.Status == (int)HttpStatusCode.NotFound)
        {
            await CreateAsync(blob, cancellationToken);
            await AppendBlockAsync(blob, lines, cancellationToken);
        }
    }

    private static async Task CreateAsync(AppendBlobClient blob, CancellationToken cancellationToken)
    {
        try
        {
            await blob.CreateAsync(new AppendBlobCreateOptions { Conditions = new AppendBlobRequestConditions { IfNoneMatch = ETag.All } }, cancellationToken);
        }
        catch (RequestFailedException exception) when (exception.Status == (int)HttpStatusCode.Conflict)
        {
        }
    }

    private static async Task AppendBlockAsync(AppendBlobClient blob, byte[] lines, CancellationToken cancellationToken)
    {
        using var content = new MemoryStream(lines);
        await blob.AppendBlockAsync(content, cancellationToken: cancellationToken);
    }

    private static byte[] LinesOf(IEnumerable<SecurityEvent> entries)
    {
        var lines = new StringBuilder();
        foreach (var entry in entries)
        {
            lines.Append(JsonSerializer.Serialize(entry, Options)).Append(LineSeparator);
        }

        return Encoding.UTF8.GetBytes(lines.ToString());
    }
}
