using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.UseCases.Announcements;

public sealed record CreateAnnouncementCommand(
    string? Title,
    string? Body,
    AnnouncementCategory? Category = null,
    IReadOnlyList<Guid>? ClassGroupIds = null,
    bool IsPinned = false,
    DateOnly? VisibleUntil = null,
    DateTimeOffset? PublishOn = null,
    AnnouncementAction? Action = null,
    string? ActionUrl = null) : ICommand;

public sealed record DeleteAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record RestoreAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record PinAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record UnpinAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record AddAnnouncementImageCommand(Guid AnnouncementId, byte[] Content) : ICommand;

public sealed record RemoveAnnouncementImageCommand(Guid AnnouncementId, Guid DocumentId) : ICommand;

public sealed record ListAnnouncementsQuery : IQuery;

public sealed record ListAnnouncementRecipientsQuery(Guid AnnouncementId) : IQuery;

public sealed record RemindAnnouncementCommand(Guid AnnouncementId) : ICommand;

public static class AnnouncementErrorCodes
{
    public const string NotFound = "announcement.not_found";
    public const string ClassGroupNotFound = "announcement.class_group_not_found";
    public const string ScheduledCannotBePinned = "announcement.scheduled_cannot_be_pinned";
}

internal static class AnnouncementFailures
{
    private const string NotFoundMessage = "The announcement doesn't exist.";
    private const string DeletedNotFoundMessage = "The deleted announcement doesn't exist.";
    private const string ClassGroupNotFoundMessage = "One of the classes does not exist.";
    private const string ScheduledCannotBePinnedMessage = "A scheduled announcement can be pinned once it is published.";

    public static ResultError NotFound() => new(AnnouncementErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError DeletedNotFound() => new(AnnouncementErrorCodes.NotFound, DeletedNotFoundMessage, ErrorKind.NotFound);

    public static ResultError ScheduledCannotBePinned() =>
        new(AnnouncementErrorCodes.ScheduledCannotBePinned, ScheduledCannotBePinnedMessage, ErrorKind.Validation)
        {
            FieldName = nameof(CreateAnnouncementCommand.IsPinned),
        };

    public static ResultError ClassGroupNotFound() =>
        new(AnnouncementErrorCodes.ClassGroupNotFound, ClassGroupNotFoundMessage, ErrorKind.Validation)
        {
            FieldName = nameof(CreateAnnouncementCommand.ClassGroupIds),
        };
}

public sealed class CreateAnnouncementUseCase(
    IAnnouncementRepository announcementRepository,
    IClassGroupRepository classGroupRepository,
    IAnnouncementResponseService responses,
    IUnitOfWork unitOfWork,
    IStudentAppNotificationService studentAppNotifications,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<CreateAnnouncementCommand, AnnouncementResponse>
{
    public async Task<Result<AnnouncementResponse>> ExecuteAsync(CreateAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var isScheduled = command.PublishOn > now;
        if (isScheduled && command.IsPinned)
        {
            return AnnouncementFailures.ScheduledCannotBePinned();
        }

        var announcement = Announcement.Create(
            command.Title,
            command.Body,
            isScheduled ? command.PublishOn!.Value : now,
            await businessCalendar.TodayAsync(cancellationToken),
            command.Category ?? AnnouncementCategory.News,
            command.ClassGroupIds,
            command.VisibleUntil,
            command.Action ?? AnnouncementAction.None,
            command.ActionUrl);
        if (announcement.IsFailure)
        {
            return announcement.Error!;
        }

        foreach (var classGroupId in announcement.Value!.ClassGroupIds)
        {
            if (await classGroupRepository.GetByIdAsync(classGroupId, cancellationToken) is null)
            {
                return AnnouncementFailures.ClassGroupNotFound();
            }
        }

        if (command.IsPinned)
        {
            await AnnouncementPins.UnpinCurrentAsync(announcementRepository, unitOfWork, cancellationToken);
            announcement.Value.Pin();
        }

        announcementRepository.Add(announcement.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await studentAppNotifications.AnnouncementPublishedAsync(announcement.Value, cancellationToken);
        return await responses.FromAsync(announcement.Value, cancellationToken);
    }
}

public sealed class ListAnnouncementsUseCase(
    IAnnouncementRepository announcementRepository,
    IAnnouncementResponseService responses,
    TimeProvider timeProvider)
    : IUseCase<ListAnnouncementsQuery, IReadOnlyList<AnnouncementResponse>>
{
    public const int ListedDays = 180;

    public async Task<Result<IReadOnlyList<AnnouncementResponse>>> ExecuteAsync(ListAnnouncementsQuery command, CancellationToken cancellationToken)
    {
        var announcements = await announcementRepository.ListRecentOrPinnedAsync(
            timeProvider.GetUtcNow().AddDays(-ListedDays), cancellationToken);
        return Result.Success(await responses.ListFromAsync(announcements, cancellationToken));
    }
}

public sealed class DeleteAnnouncementUseCase(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork)
    : IUseCase<DeleteAnnouncementCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(DeleteAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        announcementRepository.Remove(announcement);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class RestoreAnnouncementUseCase(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork)
    : IUseCase<RestoreAnnouncementCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RestoreAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindDeletedForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.DeletedNotFound();
        }

        if (announcement.IsPinned && await announcementRepository.FindPinnedForUpdateAsync(cancellationToken) is not null)
        {
            announcement.Unpin();
        }

        announcement.Restore();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class PinAnnouncementUseCase(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IUseCase<PinAnnouncementCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(PinAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        if (announcement.IsPinned)
        {
            return true;
        }

        if (!announcement.IsPublishedBy(timeProvider.GetUtcNow()))
        {
            return AnnouncementFailures.ScheduledCannotBePinned();
        }

        await AnnouncementPins.UnpinCurrentAsync(announcementRepository, unitOfWork, cancellationToken);
        announcement.Pin();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class UnpinAnnouncementUseCase(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork)
    : IUseCase<UnpinAnnouncementCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(UnpinAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        announcement.Unpin();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class AddAnnouncementImageUseCase(
    IAnnouncementRepository announcementRepository,
    IAnnouncementResponseService responses,
    ICatalogImageService catalogImageService)
    : IUseCase<AddAnnouncementImageCommand, AnnouncementResponse>
{
    public Task<Result<AnnouncementResponse>> ExecuteAsync(AddAnnouncementImageCommand command, CancellationToken cancellationToken) =>
        AnnouncementImageEditing.ChangeAsync(
            announcementRepository,
            responses,
            command.AnnouncementId,
            announcement => catalogImageService.AddAsync(announcement, DocumentOwner.Announcement, command.Content, cancellationToken),
            cancellationToken);
}

public sealed class RemoveAnnouncementImageUseCase(
    IAnnouncementRepository announcementRepository,
    IAnnouncementResponseService responses,
    ICatalogImageService catalogImageService)
    : IUseCase<RemoveAnnouncementImageCommand, AnnouncementResponse>
{
    public Task<Result<AnnouncementResponse>> ExecuteAsync(RemoveAnnouncementImageCommand command, CancellationToken cancellationToken) =>
        AnnouncementImageEditing.ChangeAsync(
            announcementRepository,
            responses,
            command.AnnouncementId,
            announcement => catalogImageService.RemoveAsync(announcement, command.DocumentId, cancellationToken),
            cancellationToken);
}

internal static class AnnouncementPins
{
    public static async Task UnpinCurrentAsync(
        IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        if (await announcementRepository.FindPinnedForUpdateAsync(cancellationToken) is not { } pinned)
        {
            return;
        }

        pinned.Unpin();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static class AnnouncementImageEditing
{
    public static async Task<Result<AnnouncementResponse>> ChangeAsync(
        IAnnouncementRepository announcementRepository,
        IAnnouncementResponseService responses,
        Guid announcementId,
        Func<Announcement, Task<Result>> change,
        CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(announcementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        var result = await change(announcement);
        if (result.IsFailure)
        {
            return result.Error!;
        }

        return await responses.FromAsync(announcement, cancellationToken);
    }
}

public sealed class ListAnnouncementRecipientsUseCase(
    IAnnouncementRepository announcementRepository,
    IAnnouncementAudienceService audienceService)
    : IUseCase<ListAnnouncementRecipientsQuery, IReadOnlyList<AnnouncementRecipientResponse>>
{
    public async Task<Result<IReadOnlyList<AnnouncementRecipientResponse>>> ExecuteAsync(
        ListAnnouncementRecipientsQuery command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        var recipients = (await audienceService.RecipientsOfAsync([announcement], cancellationToken))[announcement.Id];
        var seenAtByClient = (await announcementRepository.ListViewsAsync([announcement.Id], cancellationToken))
            .ToDictionary(view => view.ClientId, view => view.SeenAt);
        var isGoingByClient = (await announcementRepository.ListRepliesAsync([announcement.Id], cancellationToken))
            .ToDictionary(reply => reply.ClientId, reply => reply.IsGoing);
        return Result.Success<IReadOnlyList<AnnouncementRecipientResponse>>(
        [
            .. recipients.Select(recipient => new AnnouncementRecipientResponse(
                recipient.ClientId,
                recipient.FullName,
                seenAtByClient.TryGetValue(recipient.ClientId, out var seenAt) ? seenAt : null,
                isGoingByClient.TryGetValue(recipient.ClientId, out var isGoing) ? isGoing : null)),
        ]);
    }
}

public sealed class RemindAnnouncementUseCase(
    IAnnouncementRepository announcementRepository,
    IAnnouncementAudienceService audienceService,
    IStudentAppNotificationService studentAppNotifications)
    : IUseCase<RemindAnnouncementCommand, AnnouncementReminderResponse>
{
    public async Task<Result<AnnouncementReminderResponse>> ExecuteAsync(RemindAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.FindForUpdateAsync(command.AnnouncementId, cancellationToken);
        if (announcement is null)
        {
            return AnnouncementFailures.NotFound();
        }

        var viewerIds = (await announcementRepository.ListViewsAsync([announcement.Id], cancellationToken))
            .Select(view => view.ClientId)
            .ToHashSet();
        var unseenClientIds = (await audienceService.RecipientsOfAsync([announcement], cancellationToken))[announcement.Id]
            .Select(recipient => recipient.ClientId)
            .Where(clientId => !viewerIds.Contains(clientId))
            .ToList();
        if (unseenClientIds.Count > 0)
        {
            await studentAppNotifications.AnnouncementReminderAsync(announcement, unseenClientIds, cancellationToken);
        }

        return new AnnouncementReminderResponse(unseenClientIds.Count);
    }
}
