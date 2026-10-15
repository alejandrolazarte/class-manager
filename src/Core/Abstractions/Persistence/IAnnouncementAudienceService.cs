using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record AnnouncementRecipient(Guid ClientId, string FullName);

public interface IAnnouncementAudienceService
{
    Task<IReadOnlyDictionary<Guid, IReadOnlyList<AnnouncementRecipient>>> RecipientsOfAsync(
        IReadOnlyList<Announcement> announcements, CancellationToken cancellationToken);
}
