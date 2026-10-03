using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Documents;

public sealed class Document : ITenantOwned
{
    public const int PathMaxLength = 300;
    public const int ContentTypeMaxLength = 64;

    private Document()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Path { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public DocumentVisibility Visibility { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Document Create(
        Guid id,
        string path,
        DocumentContent content,
        DocumentVisibility visibility,
        DateTimeOffset createdAt) =>
        new()
        {
            Id = id,
            Path = path,
            ContentType = content.ContentType,
            SizeInBytes = content.Content.LongLength,
            Visibility = visibility,
            CreatedAt = createdAt.ToUniversalTime(),
        };
}
