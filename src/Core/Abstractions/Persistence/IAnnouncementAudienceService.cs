using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.Abstractions.Persistence;

public sealed record AnnouncementRecipient(long ClientId, string FullName);

public interface IAnnouncementAudienceService
{
    Task<IReadOnlyDictionary<long, IReadOnlyList<AnnouncementRecipient>>> RecipientsOfAsync(
        IReadOnlyList<Announcement> announcements, CancellationToken cancellationToken);
}
