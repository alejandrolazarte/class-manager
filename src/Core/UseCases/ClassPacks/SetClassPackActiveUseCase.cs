using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record SetClassPackActiveCommand(Guid ClassPackId, bool IsActive) : ICommand;

public sealed class SetClassPackActiveUseCase(
    IClassPackRepository classPackRepository,
    IUnitOfWork unitOfWork,
    IDocumentStorageService documentStorage)
    : IUseCase<SetClassPackActiveCommand, ClassPackResponse>
{
    public async Task<Result<ClassPackResponse>> ExecuteAsync(SetClassPackActiveCommand command, CancellationToken cancellationToken)
    {
        var classPack = await classPackRepository.GetForUpdateAsync(command.ClassPackId, cancellationToken);
        if (classPack is null)
        {
            return ClassPackFailures.NotFound();
        }

        if (command.IsActive)
        {
            classPack.Activate();
        }
        else
        {
            classPack.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassPackResponse.From(classPack, documentStorage);
    }
}
