using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Users;

namespace ClassManager.Core.UseCases.UserPhotos;

public sealed record GetMyPhotoQuery : IQuery;

public sealed record ChangeMyPhotoCommand(byte[] Content) : ICommand;

public sealed record RemoveMyPhotoCommand : ICommand;

public sealed record ListPhotoApprovalsQuery : IQuery;

public sealed record ApprovePhotoCommand(long PhotoId) : ICommand;

public sealed record RefusePhotoCommand(long PhotoId) : ICommand;

public sealed record MyPhotoResponse(string? Url, bool AwaitsGuardian, bool NeedsGuardian);

public sealed record PhotoApprovalResponse(long Id, string StudentFullName, string PhotoUrl);

public sealed record PhotoDecisionResponse;

internal static class UserPhotoRules
{
    private const string NoUserMessage = "This account has no person in this business.";
    private const string PhotoNotFoundMessage = "There is no photo to remove.";
    private const string ApprovalNotFoundMessage = "There is no photo waiting for your approval with this id.";

    public static Result<TResponse> NoUser<TResponse>() =>
        Result.Forbidden<TResponse>(NoUserMessage, UserPhotoErrorCodes.NoUser);

    public static Result<TResponse> PhotoNotFound<TResponse>() =>
        Result.NotFound<TResponse>(PhotoNotFoundMessage, UserPhotoErrorCodes.NotFound);

    public static Result<TResponse> ApprovalNotFound<TResponse>() =>
        Result.NotFound<TResponse>(ApprovalNotFoundMessage, UserPhotoErrorCodes.NotFound);

    public static async Task<bool> NeedsGuardianAsync(IStudentAppFamilyService familyService, CancellationToken cancellationToken) =>
        (await familyService.GetAsync(cancellationToken))?.OwnStudentNeedsGuardian ?? false;

    public static MyPhotoResponse ResponseFor(UserPhoto? photo, bool needsGuardian, IDocumentStorageService documentStorage) =>
        new(photo is null ? null : documentStorage.PrivateUrlOf(photo.Document).ToString(), photo?.AwaitsGuardian ?? false, needsGuardian);

    public static async Task<UserPhoto?> FindAwaitingOfFamilyAsync(
        IStudentAppFamilyService familyService,
        IUserPhotoRepository photoRepository,
        long photoId,
        CancellationToken cancellationToken)
    {
        var family = await familyService.GetAsync(cancellationToken);
        if (family is null || !family.Access.PaysForFamily)
        {
            return null;
        }

        var photo = await photoRepository.FindForUpdateAsync(photoId, cancellationToken);
        return photo is { AwaitsGuardian: true } && GuardedUserIdsOf(family).Contains(photo.UserId) ? photo : null;
    }

    public static IReadOnlyCollection<long> GuardedUserIdsOf(StudentAppFamily family) =>
        [.. family.EveryStudent.Select(student => student.UserId).Where(userId => userId != family.Client.UserId).Distinct()];
}

public sealed class GetMyPhotoUseCase(
    ICurrentUser currentUser,
    IUserPhotoRepository photoRepository,
    IStudentAppFamilyService familyService,
    IDocumentStorageService documentStorage)
    : IUseCase<GetMyPhotoQuery, MyPhotoResponse>
{
    public async Task<Result<MyPhotoResponse>> ExecuteAsync(GetMyPhotoQuery command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return UserPhotoRules.NoUser<MyPhotoResponse>();
        }

        var photo = await photoRepository.FindByUserAsync(userId, cancellationToken);
        return UserPhotoRules.ResponseFor(photo, await UserPhotoRules.NeedsGuardianAsync(familyService, cancellationToken), documentStorage);
    }
}

public sealed class ChangeMyPhotoUseCase(
    ICurrentUser currentUser,
    IUserPhotoRepository photoRepository,
    IDocumentRepository documentRepository,
    IStudentAppFamilyService familyService,
    IAvatarImageService avatarImageService,
    IDocumentStorageService documentStorage,
    IStudentAppNotificationService studentAppNotificationService,
    IUnitOfWork unitOfWork)
    : IUseCase<ChangeMyPhotoCommand, MyPhotoResponse>
{
    public async Task<Result<MyPhotoResponse>> ExecuteAsync(ChangeMyPhotoCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return UserPhotoRules.NoUser<MyPhotoResponse>();
        }

        var upload = UserPhotoUpload.Validate(command.Content);
        var avatar = upload.IsSuccess ? avatarImageService.ToAvatar(command.Content, UserPhoto.SideInPixels) : null;
        if (avatar is null)
        {
            return (upload.IsFailure ? upload : UserPhotoUpload.Unsupported()).Error!;
        }

        var family = await familyService.GetAsync(cancellationToken);
        var needsGuardian = family?.OwnStudentNeedsGuardian ?? false;
        var previousPhoto = await photoRepository.FindByUserAsync(userId, cancellationToken);
        var document = await documentStorage.StoreAsync(DocumentOwner.User, userId, avatar, DocumentVisibility.Private, cancellationToken);
        if (previousPhoto is not null)
        {
            photoRepository.Remove(previousPhoto);
            documentRepository.Remove(previousPhoto.Document);
        }

        documentRepository.Add(document);
        var photo = UserPhoto.Create(userId, document, needsGuardian);
        photoRepository.Add(photo);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await documentStorage.DeleteFileAsync(document, CancellationToken.None);
            throw;
        }

        if (previousPhoto is not null)
        {
            await documentStorage.DeleteFileAsync(previousPhoto.Document, cancellationToken);
        }

        if (needsGuardian && family!.Members.FirstOrDefault(member => member.Student.Id == family.Access.StudentId) is { } ownMember)
        {
            await studentAppNotificationService.PhotoApprovalRequestedAsync(family.Client.Id, ownMember.Student.User.FullName, cancellationToken);
        }

        return UserPhotoRules.ResponseFor(photo, needsGuardian, documentStorage);
    }
}

public sealed class RemoveMyPhotoUseCase(
    ICurrentUser currentUser,
    IUserPhotoRepository photoRepository,
    IDocumentRepository documentRepository,
    IDocumentStorageService documentStorage,
    IUnitOfWork unitOfWork)
    : IUseCase<RemoveMyPhotoCommand, PhotoDecisionResponse>
{
    public async Task<Result<PhotoDecisionResponse>> ExecuteAsync(RemoveMyPhotoCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not { } userId)
        {
            return UserPhotoRules.NoUser<PhotoDecisionResponse>();
        }

        var photo = await photoRepository.FindByUserAsync(userId, cancellationToken);
        if (photo is null)
        {
            return UserPhotoRules.PhotoNotFound<PhotoDecisionResponse>();
        }

        photoRepository.Remove(photo);
        documentRepository.Remove(photo.Document);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await documentStorage.DeleteFileAsync(photo.Document, cancellationToken);
        return new PhotoDecisionResponse();
    }
}

public sealed class ListPhotoApprovalsUseCase(
    IStudentAppFamilyService familyService,
    IUserPhotoRepository photoRepository,
    IDocumentStorageService documentStorage)
    : IUseCase<ListPhotoApprovalsQuery, IReadOnlyList<PhotoApprovalResponse>>
{
    public async Task<Result<IReadOnlyList<PhotoApprovalResponse>>> ExecuteAsync(ListPhotoApprovalsQuery command, CancellationToken cancellationToken)
    {
        var family = await familyService.GetAsync(cancellationToken);
        if (family is null || !family.Access.PaysForFamily)
        {
            return Result.Success<IReadOnlyList<PhotoApprovalResponse>>([]);
        }

        var photos = await photoRepository.ListAwaitingGuardianAsync(UserPhotoRules.GuardedUserIdsOf(family), cancellationToken);
        var namesByUserId = family.EveryStudent
            .GroupBy(student => student.UserId)
            .ToDictionary(students => students.Key, students => students.First().User.FullName);
        return Result.Success<IReadOnlyList<PhotoApprovalResponse>>(
        [
            .. photos.Select(photo => new PhotoApprovalResponse(
                photo.Id,
                namesByUserId[photo.UserId],
                documentStorage.PrivateUrlOf(photo.Document).ToString())),
        ]);
    }
}

public sealed class ApprovePhotoUseCase(
    IStudentAppFamilyService familyService,
    IUserPhotoRepository photoRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<ApprovePhotoCommand, PhotoDecisionResponse>
{
    public async Task<Result<PhotoDecisionResponse>> ExecuteAsync(ApprovePhotoCommand command, CancellationToken cancellationToken)
    {
        var photo = await UserPhotoRules.FindAwaitingOfFamilyAsync(familyService, photoRepository, command.PhotoId, cancellationToken);
        if (photo is null)
        {
            return UserPhotoRules.ApprovalNotFound<PhotoDecisionResponse>();
        }

        photo.ApproveByGuardian();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new PhotoDecisionResponse();
    }
}

public sealed class RefusePhotoUseCase(
    IStudentAppFamilyService familyService,
    IUserPhotoRepository photoRepository,
    IDocumentRepository documentRepository,
    IDocumentStorageService documentStorage,
    IUnitOfWork unitOfWork)
    : IUseCase<RefusePhotoCommand, PhotoDecisionResponse>
{
    public async Task<Result<PhotoDecisionResponse>> ExecuteAsync(RefusePhotoCommand command, CancellationToken cancellationToken)
    {
        var photo = await UserPhotoRules.FindAwaitingOfFamilyAsync(familyService, photoRepository, command.PhotoId, cancellationToken);
        if (photo is null)
        {
            return UserPhotoRules.ApprovalNotFound<PhotoDecisionResponse>();
        }

        photoRepository.Remove(photo);
        documentRepository.Remove(photo.Document);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await documentStorage.DeleteFileAsync(photo.Document, cancellationToken);
        return new PhotoDecisionResponse();
    }
}
