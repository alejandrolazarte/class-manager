using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class Announcement : ITenantOwned, ISoftDeletable
{
    public const int TitleMaxLength = 80;
    public const int BodyMaxLength = 500;

    private const string TitleRequiredMessage = "Write a title.";
    private const string TitleLengthMessage = "The title must be at most 80 characters.";
    private const string BodyLengthMessage = "The text must be at most 500 characters.";

    private Announcement()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Body { get; private set; }
    public DateTimeOffset PublishedAt { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<Announcement> Create(string? title, string? body, DateTimeOffset publishedAt)
    {
        var trimmedTitle = title?.Trim() ?? string.Empty;
        if (trimmedTitle.Length == 0)
        {
            return Result.Validation<Announcement>(TitleRequiredMessage, fieldName: nameof(Title));
        }

        if (trimmedTitle.Length > TitleMaxLength)
        {
            return Result.Validation<Announcement>(TitleLengthMessage, fieldName: nameof(Title));
        }

        var trimmedBody = string.IsNullOrWhiteSpace(body) ? null : body.Trim();
        if (trimmedBody?.Length > BodyMaxLength)
        {
            return Result.Validation<Announcement>(BodyLengthMessage, fieldName: nameof(Body));
        }

        return new Announcement
        {
            Id = Guid.CreateVersion7(),
            Title = trimmedTitle,
            Body = trimmedBody,
            PublishedAt = publishedAt.ToUniversalTime(),
        };
    }

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }
}
