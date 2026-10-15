using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
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
    DateOnly? VisibleUntil = null) : ICommand;

public sealed record DeleteAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record RestoreAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record PinAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record UnpinAnnouncementCommand(Guid AnnouncementId) : ICommand;

public sealed record AddAnnouncementImageCommand(Guid AnnouncementId, byte[] Content) : ICommand;

public sealed record RemoveAnnouncementImageCommand(Guid AnnouncementId, Guid DocumentId) : ICommand;

public sealed record ListAnnouncementsQuery : IQuery;

public static class AnnouncementErrorCodes
{
    public const string NotFound = "announcement.not_found";
    public const string ClassGroupNotFound = "announcement.class_group_not_found";
}

internal static class AnnouncementFailures
{
    private const string NotFoundMessage = "The announcement doesn't exist.";
    private const string DeletedNotFoundMessage = "The deleted announcement doesn't exist.";
    private const string ClassGroupNotFoundMessage = "One of the classes does not exist.";

    public static ResultError NotFound() => new(AnnouncementErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError DeletedNotFound() => new(AnnouncementErrorCodes.NotFound, DeletedNotFoundMessage, ErrorKind.NotFound);

    public static ResultError ClassGroupNotFound() =>
        new(AnnouncementErrorCodes.ClassGroupNotFound, ClassGroupNotFoundMessage, ErrorKind.Validation)
        {
            FieldName = nameof(CreateAnnouncementCommand.ClassGroupIds),
        };
}

public sealed class CreateAnnouncementUseCase(
    IAnnouncementRepository announcementRepository,
    IClassGroupRepository classGroupRepository,
    IDocumentStorageService documentStorage,
    IUnitOfWork unitOfWork,
    IStudentAppNotificationService studentAppNotifications,
    IBusinessCalendarService businessCalendar,
    TimeProvider timeProvider)
    : IUseCase<CreateAnnouncementCommand, AnnouncementResponse>
{
    public async Task<Result<AnnouncementResponse>> ExecuteAsync(CreateAnnouncementCommand command, CancellationToken cancellationToken)
    {
        var announcement = Announcement.Create(
            command.Title,
            command.Body,
            timeProvider.GetUtcNow(),
            await businessCalendar.TodayAsync(cancellationToken),
            command.Category ?? AnnouncementCategory.News,
            command.ClassGroupIds,
            command.VisibleUntil);
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
        return await AnnouncementResponse.FromAsync(announcement.Value, classGroupRepository, documentStorage, cancellationToken);
    }
}

public sealed class ListAnnouncementsUseCase(
    IAnnouncementRepository announcementRepository,
    IClassGroupRepository classGroupRepository,
    IDocumentStorageService documentStorage,
    TimeProvider timeProvider)
    : IUseCase<ListAnnouncementsQuery, IReadOnlyList<AnnouncementResponse>>
{
    public const int ListedDays = 180;

    public async Task<Result<IReadOnlyList<AnnouncementResponse>>> ExecuteAsync(ListAnnouncementsQuery command, CancellationToken cancellationToken)
    {
        var announcements = await announcementRepository.ListRecentOrPinnedAsync(
            timeProvider.GetUtcNow().AddDays(-ListedDays), cancellationToken);
        return Result.Success(await AnnouncementResponse.ListFromAsync(announcements, classGroupRepository, documentStorage, cancellationToken));
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

public sealed class PinAnnouncementUseCase(IAnnouncementRepository announcementRepository, IUnitOfWork unitOfWork)
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
    IClassGroupRepository classGroupRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<AddAnnouncementImageCommand, AnnouncementResponse>
{
    public Task<Result<AnnouncementResponse>> ExecuteAsync(AddAnnouncementImageCommand command, CancellationToken cancellationToken) =>
        AnnouncementImageEditing.ChangeAsync(
            announcementRepository,
            classGroupRepository,
            documentStorage,
            command.AnnouncementId,
            announcement => catalogImageService.AddAsync(announcement, DocumentOwner.Announcement, command.Content, cancellationToken),
            cancellationToken);
}

public sealed class RemoveAnnouncementImageUseCase(
    IAnnouncementRepository announcementRepository,
    IClassGroupRepository classGroupRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<RemoveAnnouncementImageCommand, AnnouncementResponse>
{
    public Task<Result<AnnouncementResponse>> ExecuteAsync(RemoveAnnouncementImageCommand command, CancellationToken cancellationToken) =>
        AnnouncementImageEditing.ChangeAsync(
            announcementRepository,
            classGroupRepository,
            documentStorage,
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
        IClassGroupRepository classGroupRepository,
        IDocumentStorageService documentStorage,
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

        return await AnnouncementResponse.FromAsync(announcement, classGroupRepository, documentStorage, cancellationToken);
    }
}
