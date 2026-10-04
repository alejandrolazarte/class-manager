using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record ListClassPacksQuery(bool IncludeInactive) : IQuery;

public sealed class ListClassPacksUseCase(IClassPackRepository classPackRepository, IDocumentStorageService documentStorage)
    : IUseCase<ListClassPacksQuery, IReadOnlyList<ClassPackResponse>>
{
    public async Task<Result<IReadOnlyList<ClassPackResponse>>> ExecuteAsync(ListClassPacksQuery command, CancellationToken cancellationToken)
    {
        var classPacks = await classPackRepository.ListAsync(command.IncludeInactive, cancellationToken);

        return Result.Success<IReadOnlyList<ClassPackResponse>>([.. classPacks.Select(classPack => ClassPackResponse.From(classPack, documentStorage))]);
    }
}
