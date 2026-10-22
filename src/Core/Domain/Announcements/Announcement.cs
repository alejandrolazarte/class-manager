using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Announcements;

public sealed class Announcement : ITenantOwned, IAuditable, ISoftDeletable, IHasCatalogImages
{
    public const int TitleMaxLength = 80;
    public const int BodyMaxLength = 1000;
    public const int MaximumImages = 1;
    public const int ActionUrlMaxLength = 500;

    private readonly List<AnnouncementClassGroup> _classGroups = [];
    private readonly List<AnnouncementImage> _images = [];

    private Announcement()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Body { get; private set; }
    public AnnouncementCategory Category { get; private set; }
    public bool IsPinned { get; private set; }
    public DateOnly? VisibleUntil { get; private set; }
    public AnnouncementAction Action { get; private set; }
    public string? ActionUrl { get; private set; }
    public DateTimeOffset PublishedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }
    public IReadOnlyList<AnnouncementClassGroup> ClassGroups => _classGroups;
    public IReadOnlyList<long> ClassGroupIds => [.. _classGroups.Select(classGroup => classGroup.ClassGroupId)];
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
        IReadOnlyCollection<long>? classGroupIds = null,
        DateOnly? visibleUntil = null,
        AnnouncementAction action = AnnouncementAction.None,
        string? actionUrl = null)
    {
        var content = AnnouncementContent.From(title, body, category, visibleUntil, today, action, actionUrl);
        if (content.IsFailure)
        {
            return content.Error!;
        }

        var announcement = new Announcement
        {
            PublishedAt = publishedAt.ToUniversalTime(),
        };
        announcement.Apply(content.Value!, classGroupIds);
        return announcement;
    }

    public Result Edit(
        string? title,
        string? body,
        AnnouncementCategory category,
        IReadOnlyCollection<long>? classGroupIds,
        DateOnly? visibleUntil,
        DateOnly today,
        AnnouncementAction action = AnnouncementAction.None,
        string? actionUrl = null)
    {
        var content = AnnouncementContent.From(
            title, body, category, visibleUntil == VisibleUntil ? null : visibleUntil, today, action, actionUrl);
        if (content.IsFailure)
        {
            return Result.Failure(content.Error!);
        }

        Apply(content.Value! with { VisibleUntil = visibleUntil }, classGroupIds);
        return Result.Success();
    }

    public void Reschedule(DateTimeOffset publishedAt) => PublishedAt = publishedAt.ToUniversalTime();

    public bool IsFor(IReadOnlyCollection<long> enrolledClassGroupIds) =>
        IsForEveryone || _classGroups.Any(classGroup => enrolledClassGroupIds.Contains(classGroup.ClassGroupId));

    public bool IsPublishedBy(DateTimeOffset now) => PublishedAt <= now;

    public bool IsVisibleOn(DateOnly date) => VisibleUntil is not { } lastDay || date <= lastDay;

    public void AddImage(Document document) => _images.Add(AnnouncementImage.Create(Id, document, _images.Count));

    public Document? RemoveImage(long documentId) => CatalogImageGallery.Remove(_images, documentId);

    public Result ReorderImages(IReadOnlyList<long>? documentIds) => CatalogImageGallery.Reorder(_images, documentIds);

    public void Pin() => IsPinned = true;

    public void Unpin() => IsPinned = false;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    public void Restore()
    {
        DeletedAt = null;
        DeletedByUserId = null;
        IsDeleted = false;
    }

    private void Apply(AnnouncementContent content, IReadOnlyCollection<long>? classGroupIds)
    {
        Title = content.Title;
        Body = content.Body;
        Category = content.Category;
        VisibleUntil = content.VisibleUntil;
        Action = content.Action;
        ActionUrl = content.ActionUrl;
        var wantedClassGroupIds = (classGroupIds ?? []).Distinct().ToList();
        _classGroups.RemoveAll(classGroup => !wantedClassGroupIds.Contains(classGroup.ClassGroupId));
        foreach (var classGroupId in wantedClassGroupIds.Where(classGroupId => _classGroups.All(classGroup => classGroup.ClassGroupId != classGroupId)))
        {
            _classGroups.Add(AnnouncementClassGroup.Create(Id, classGroupId));
        }
    }
}
