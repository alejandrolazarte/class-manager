using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Images;

namespace ClassManager.Core.Abstractions.Images;

public interface ICatalogImageService
{
    Task<Result> AddAsync(IHasCatalogImages imageHolder, DocumentOwner owner, byte[] content, CancellationToken cancellationToken);

    Task<Result> RemoveAsync(IHasCatalogImages imageHolder, Guid documentId, CancellationToken cancellationToken);

    Task<Result> ReorderAsync(IHasCatalogImages imageHolder, IReadOnlyList<Guid>? documentIds, CancellationToken cancellationToken);
}
