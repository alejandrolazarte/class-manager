using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class Announcement : ITenantOwned, ISoftDeletable, IHasCatalogImages
{
    public const int TitleMaxLength = 80;
    public const int BodyMaxLength = 1000;
    public const int MaximumImages = 1;

    private const string TitleRequiredMessage = "Write a title.";
    private const string TitleLengthMessage = "The title must be at most 80 characters.";
    private const string BodyLengthMessage = "The text must be at most 1000 characters.";
    private const string CategoryMessage = "Choose the kind of announcement.";
    private const string VisibleUntilMessage = "The announcement can't be hidden before today.";

    private readonly List<AnnouncementClassGroup> _classGroups = [];
    private readonly List<AnnouncementImage> _images = [];

    private Announcement()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Body { get; private set; }
    public AnnouncementCategory Category { get; private set; }
    public bool IsPinned { get; private set; }
    public DateOnly? VisibleUntil { get; private set; }
    public DateTimeOffset PublishedAt { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<AnnouncementClassGroup> ClassGroups => _classGroups;
    public IReadOnlyList<Guid> ClassGroupIds => [.. _classGroups.Select(classGroup => classGroup.ClassGroupId)];
    public bool IsForEveryone => _classGroups.Count == 0;
    public IReadOnlyList<AnnouncementImage> Images => _images;
    public IReadOnlyList<Document> ImagesInOrder => CatalogImageGallery.InOrder(_images);
    public int ImageCount => _images.Count;
    public int MaximumImageCount => MaximumImages;

    public static Result<Announcement> Create(
        string? title,
        string? body,
        DateTimeOffset publishedAt,
        DateOnly today,
        AnnouncementCategory category = AnnouncementCategory.News,
        IReadOnlyCollection<Guid>? classGroupIds = null,
        DateOnly? visibleUntil = null)
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

        if (!Enum.IsDefined(category))
        {
            return Result.Validation<Announcement>(CategoryMessage, fieldName: nameof(Category));
        }

        if (visibleUntil < today)
        {
            return Result.Validation<Announcement>(VisibleUntilMessage, fieldName: nameof(VisibleUntil));
        }

        var announcement = new Announcement
        {
            Id = Guid.CreateVersion7(),
            Title = trimmedTitle,
            Body = trimmedBody,
            Category = category,
            VisibleUntil = visibleUntil,
            PublishedAt = publishedAt.ToUniversalTime(),
        };
        foreach (var classGroupId in (classGroupIds ?? []).Distinct())
        {
            announcement._classGroups.Add(AnnouncementClassGroup.Create(announcement.Id, classGroupId));
        }

        return announcement;
    }

    public bool IsFor(IReadOnlyCollection<Guid> enrolledClassGroupIds) =>
        IsForEveryone || _classGroups.Any(classGroup => enrolledClassGroupIds.Contains(classGroup.ClassGroupId));

    public bool IsPublishedBy(DateTimeOffset now) => PublishedAt <= now;

    public bool IsVisibleOn(DateOnly date) => VisibleUntil is not { } lastDay || date <= lastDay;

    public void AddImage(Document document) => _images.Add(AnnouncementImage.Create(Id, document, _images.Count));

    public Document? RemoveImage(Guid documentId) => CatalogImageGallery.Remove(_images, documentId);

    public Result ReorderImages(IReadOnlyList<Guid>? documentIds) => CatalogImageGallery.Reorder(_images, documentIds);

    public void Pin() => IsPinned = true;

    public void Unpin() => IsPinned = false;

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }

    public void Restore()
    {
        DeletedOn = null;
        IsDeleted = false;
    }
}
