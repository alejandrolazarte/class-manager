using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record AddClassPackImageCommand(long ClassPackId, byte[] Content) : ICommand;

public sealed record RemoveClassPackImageCommand(long ClassPackId, long DocumentId) : ICommand;

public sealed record ReorderClassPackImagesCommand(long ClassPackId, IReadOnlyList<long>? DocumentIds) : ICommand;

public sealed class AddClassPackImageUseCase(
    IClassPackRepository classPackRepository,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<AddClassPackImageCommand, ClassPackResponse>
{
    public Task<Result<ClassPackResponse>> ExecuteAsync(AddClassPackImageCommand command, CancellationToken cancellationToken) =>
        ClassPackImageEditing.ChangeAsync(
            classPackRepository,
            businessCalendar,
            documentStorage,
            command.ClassPackId,
            classPack => catalogImageService.AddAsync(classPack, DocumentOwner.ClassPack, command.Content, cancellationToken),
            cancellationToken);
}

public sealed class RemoveClassPackImageUseCase(
    IClassPackRepository classPackRepository,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<RemoveClassPackImageCommand, ClassPackResponse>
{
    public Task<Result<ClassPackResponse>> ExecuteAsync(RemoveClassPackImageCommand command, CancellationToken cancellationToken) =>
        ClassPackImageEditing.ChangeAsync(
            classPackRepository,
            businessCalendar,
            documentStorage,
            command.ClassPackId,
            classPack => catalogImageService.RemoveAsync(classPack, command.DocumentId, cancellationToken),
            cancellationToken);
}

public sealed class ReorderClassPackImagesUseCase(
    IClassPackRepository classPackRepository,
    IBusinessCalendarService businessCalendar,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<ReorderClassPackImagesCommand, ClassPackResponse>
{
    public Task<Result<ClassPackResponse>> ExecuteAsync(ReorderClassPackImagesCommand command, CancellationToken cancellationToken) =>
        ClassPackImageEditing.ChangeAsync(
            classPackRepository,
            businessCalendar,
            documentStorage,
            command.ClassPackId,
            classPack => catalogImageService.ReorderAsync(classPack, command.DocumentIds, cancellationToken),
            cancellationToken);
}

internal static class ClassPackImageEditing
{
    public static async Task<Result<ClassPackResponse>> ChangeAsync(
        IClassPackRepository classPackRepository,
        IBusinessCalendarService businessCalendar,
        IDocumentStorageService documentStorage,
        long classPackId,
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

        return await ClassPackResponses.ForAsync(classPack, classPackRepository, businessCalendar, documentStorage, cancellationToken);
    }
}
