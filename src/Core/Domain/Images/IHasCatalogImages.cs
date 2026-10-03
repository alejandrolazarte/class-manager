using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Domain.Images;

public interface IHasCatalogImages
{
    Guid Id { get; }

    int ImageCount { get; }

    void AddImage(Document document);

    Document? RemoveImage(Guid documentId);

    Result ReorderImages(IReadOnlyList<Guid>? documentIds);
}
