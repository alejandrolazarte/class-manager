using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Documents;

public sealed class Document : ITenantOwned, IAuditable
{
    public const int PathMaxLength = 300;
    public const int ContentTypeMaxLength = 64;

    private Document()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public string Path { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public DocumentVisibility Visibility { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    public static Document Create(
        string path,
        DocumentContent content,
        DocumentVisibility visibility,
        DateTimeOffset createdAt) =>
        new()
        {
            Path = path,
            ContentType = content.ContentType,
            SizeInBytes = content.Content.LongLength,
            Visibility = visibility,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
