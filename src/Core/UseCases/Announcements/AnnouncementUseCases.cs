using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Announcements;

namespace ClassManager.Core.UseCases.Announcements;

public sealed record CreateAnnouncementCommand(string? Title, string? Body);

public sealed record DeleteAnnouncementCommand(Guid AnnouncementId);

public sealed record ListAnnouncementsQuery;

public sealed record AnnouncementResponse(Guid Id, string Title, string? Body, DateTimeOffset PublishedAt)
{
    public static AnnouncementResponse From(Announcement announcement) =>
        new(announcement.Id, announcement.Title, announcement.Body, announcement.PublishedAt);
}

public static class AnnouncementErrorCodes
{
    public const string NotFound = "announcement.not_found";
}

public sealed class CreateAnnouncementUseCase(
    IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<CreateAnnouncementCommand, AnnouncementResponse>
{
    public async Task<Result<AnnouncementResponse>> ExecuteAsync(CreateAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = Announcement.Create(command.Title, command.Body, timeProvider.GetUtcNow());
        if (announcement.IsFailure)
        {
            return announcement.Error!;
        }

        announcementRepository.Add(announcement.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AnnouncementResponse.From(announcement.Value!);
    }
}

public sealed class ListAnnouncementsUseCase(IAnnouncementRepository announcementRepository, TimeProvider timeProvider)
    : IUseCase<ListAnnouncementsQuery, IReadOnlyList<AnnouncementResponse>>
{
    public const int ListedDays = 180;

    public async Task<Result<IReadOnlyList<AnnouncementResponse>>> ExecuteAsync(ListAnnouncementsQuery command, CancellationToken cancellationToken)
    {
        var announcements = await announcementRepository.ListPublishedSinceAsync(
            timeProvider.GetUtcNow().AddDays(-ListedDays), cancellationToken);
        return Result.Success<IReadOnlyList<AnnouncementResponse>>([.. announcements.Select(AnnouncementResponse.From)]);
    }
}

public sealed class DeleteAnnouncementUseCase(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork)
    : IUseCase<DeleteAnnouncementCommand, bool>
{
    private const string NotFoundMessage = "The announcement doesn't exist.";

    public async Task<Result<bool>> ExecuteAsync(DeleteAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return Result.NotFound<bool>(NotFoundMessage, AnnouncementErrorCodes.NotFound);
        }

        announcementRepository.Remove(announcement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
