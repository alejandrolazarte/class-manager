using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Images;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record SetClassPackImageCommand(Guid ClassPackId, byte[] Content);

public sealed record RemoveClassPackImageCommand(Guid ClassPackId);

public sealed class SetClassPackImageUseCase(
    IClassPackRepository classPackRepository,
    ICatalogImageService catalogImageService,
    IUnitOfWork unitOfWork)
    : IUseCase<SetClassPackImageCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(SetClassPackImageCommand command, CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetForUpdateAsync(command.ClassPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        var change = await CatalogImageChanges.ReplaceAsync(
            classPack, CatalogImageOwner.ClassPack, command.Content, catalogImageService, unitOfWork, cancellationToken);
        if (change.IsFailure)
        {
            return change.Error!;
        }

        return ClassPackResponse.From(classPack);
    }
}

public sealed class RemoveClassPackImageUseCase(
    IClassPackRepository classPackRepository,
    ICatalogImageService catalogImageService,
    IUnitOfWork unitOfWork)
    : IUseCase<RemoveClassPackImageCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(RemoveClassPackImageCommand command, CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetForUpdateAsync(command.ClassPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        await CatalogImageChanges.RemoveAsync(classPack, catalogImageService, unitOfWork, cancellationToken);

        return ClassPackResponse.From(classPack);
    }
}
