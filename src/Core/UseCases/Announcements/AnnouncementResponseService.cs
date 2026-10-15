using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.Announcements;

public interface IAnnouncementResponseService
{
    Task<AnnouncementResponse> FromAsync(Announcement announcement, CancellationToken cancellationToken);

    Task<IReadOnlyList<AnnouncementResponse>> ListFromAsync(IReadOnlyList<Announcement> announcements, CancellationToken cancellationToken);
}

public sealed class AnnouncementResponseService(
    IAnnouncementRepository announcementRepository,
    IAnnouncementAudienceService audienceService,
    IClassGroupRepository classGroupRepository,
    IDocumentStorageService documentStorage,
    TimeProvider timeProvider)
    : IAnnouncementResponseService
{
    public async Task<AnnouncementResponse> FromAsync(Announcement announcement, CancellationToken cancellationToken) =>
        (await ListFromAsync([announcement], cancellationToken))[0];

    public async Task<IReadOnlyList<AnnouncementResponse>> ListFromAsync(
        IReadOnlyList<Announcement> announcements, CancellationToken cancellationToken)
    {
        var classGroupNames = announcements.Any(announcement => !announcement.IsForEveryone)
            ? (await classGroupRepository.ListAllAsync(cancellationToken)).ToDictionary(classGroup => classGroup.Id, classGroup => classGroup.Name)
            : [];
        var now = timeProvider.GetUtcNow();
        var announcementIds = announcements.Select(announcement => announcement.Id).ToList();
        var recipients = await audienceService.RecipientsOfAsync(announcements, cancellationToken);
        var views = await announcementRepository.ListViewsAsync(announcementIds, cancellationToken);
        var replies = await announcementRepository.ListRepliesAsync(announcementIds, cancellationToken);

        return
        [
            .. announcements.Select(announcement =>
            {
                var recipientIds = recipients[announcement.Id].Select(recipient => recipient.ClientId).ToHashSet();
                var recipientReplies = replies
                    .Where(reply => reply.AnnouncementId == announcement.Id && recipientIds.Contains(reply.ClientId))
                    .ToList();
                return new AnnouncementResponse(
                    announcement.Id,
                    announcement.Title,
                    announcement.Body,
                    announcement.PublishedAt,
                    !announcement.IsPublishedBy(now),
                    announcement.Category,
                    announcement.IsPinned,
                    announcement.VisibleUntil,
                    [
                        .. announcement.ClassGroupIds
                            .Where(classGroupNames.ContainsKey)
                            .Select(classGroupId => new AnnouncementClassGroupResponse(classGroupId, classGroupNames[classGroupId])),
                    ],
                    CatalogImageResponse.ListFrom(announcement.ImagesInOrder, documentStorage),
                    recipientIds.Count,
                    views.Count(view => view.AnnouncementId == announcement.Id && recipientIds.Contains(view.ClientId)),
                    recipientReplies.Count(reply => reply.IsGoing),
                    recipientReplies.Count(reply => !reply.IsGoing));
            }),
        ];
    }
}
