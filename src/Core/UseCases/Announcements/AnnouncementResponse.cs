using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.Announcements;

public sealed record AnnouncementClassGroupResponse(Guid Id, string Name);

public sealed record AnnouncementResponse(
    Guid Id,
    string Title,
    string? Body,
    DateTimeOffset PublishedAt,
    AnnouncementCategory Category,
    bool IsPinned,
    DateOnly? VisibleUntil,
    IReadOnlyList<AnnouncementClassGroupResponse> ClassGroups,
    IReadOnlyList<CatalogImageResponse> Images)
{
    public static async Task<AnnouncementResponse> FromAsync(
        Announcement announcement,
        IClassGroupRepository classGroupRepository,
        IDocumentStorageService documentStorage,
        CancellationToken cancellationToken) =>
        (await ListFromAsync([announcement], classGroupRepository, documentStorage, cancellationToken))[0];

    public static async Task<IReadOnlyList<AnnouncementResponse>> ListFromAsync(
        IReadOnlyList<Announcement> announcements,
        IClassGroupRepository classGroupRepository,
        IDocumentStorageService documentStorage,
        CancellationToken cancellationToken)
    {
        var classGroupNames = announcements.Any(announcement => !announcement.IsForEveryone)
            ? (await classGroupRepository.ListAllAsync(cancellationToken)).ToDictionary(classGroup => classGroup.Id, classGroup => classGroup.Name)
            : [];
        return
        [
            .. announcements.Select(announcement => new AnnouncementResponse(
                announcement.Id,
                announcement.Title,
                announcement.Body,
                announcement.PublishedAt,
                announcement.Category,
                announcement.IsPinned,
                announcement.VisibleUntil,
                [
                    .. announcement.ClassGroupIds
                        .Where(classGroupNames.ContainsKey)
                        .Select(classGroupId => new AnnouncementClassGroupResponse(classGroupId, classGroupNames[classGroupId])),
                ],
                CatalogImageResponse.ListFrom(announcement.ImagesInOrder, documentStorage))),
        ];
    }
}
