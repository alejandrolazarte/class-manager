using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record AddClassPackImageCommand(Guid ClassPackId, byte[] Content) : ICommand;

public sealed record RemoveClassPackImageCommand(Guid ClassPackId, Guid DocumentId) : ICommand;

public sealed record ReorderClassPackImagesCommand(Guid ClassPackId, IReadOnlyList<Guid>? DocumentIds) : ICommand;

public sealed class AddClassPackImageUseCase(
    IClassPackRepository classPackRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<AddClassPackImageCommand, ClassPackResponse>
{
    public Task<Result<ClassPackResponse>> ExecuteAsync(AddClassPackImageCommand command, CancellationToken cancellationToken) =>
        ClassPackImageEditing.ChangeAsync(
            classPackRepository,
            documentStorage,
            command.ClassPackId,
            classPack => catalogImageService.AddAsync(classPack, DocumentOwner.ClassPack, command.Content, cancellationToken),
            cancellationToken);
}

public sealed class RemoveClassPackImageUseCase(
    IClassPackRepository classPackRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<RemoveClassPackImageCommand, ClassPackResponse>
{
    public Task<Result<ClassPackResponse>> ExecuteAsync(RemoveClassPackImageCommand command, CancellationToken cancellationToken) =>
        ClassPackImageEditing.ChangeAsync(
            classPackRepository,
            documentStorage,
            command.ClassPackId,
            classPack => catalogImageService.RemoveAsync(classPack, command.DocumentId, cancellationToken),
            cancellationToken);
}

public sealed class ReorderClassPackImagesUseCase(
    IClassPackRepository classPackRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<ReorderClassPackImagesCommand, ClassPackResponse>
{
    public Task<Result<ClassPackResponse>> ExecuteAsync(ReorderClassPackImagesCommand command, CancellationToken cancellationToken) =>
        ClassPackImageEditing.ChangeAsync(
            classPackRepository,
            documentStorage,
            command.ClassPackId,
            classPack => catalogImageService.ReorderAsync(classPack, command.DocumentIds, cancellationToken),
            cancellationToken);
}

internal static class ClassPackImageEditing
{
    public static async Task<Result<ClassPackResponse>> ChangeAsync(
        IClassPackRepository classPackRepository,
        IDocumentStorageService documentStorage,
        Guid classPackId,
        Func<ClassPack, Task<Result>> change,
        CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetForUpdateAsync(classPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        var result = await change(classPack);
        if (result.IsFailure)
        {
            return result.Error!;
        }

        return ClassPackResponse.From(classPack, documentStorage);
    }
}
