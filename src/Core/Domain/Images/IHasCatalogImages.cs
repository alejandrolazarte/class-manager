using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Domain.Images;

public interface IHasCatalogImages
{
    long Id { get; }

    int ImageCount { get; }

    int MaximumImageCount { get; }

    void AddImage(Document document);

    Document? RemoveImage(long documentId);

    Result ReorderImages(IReadOnlyList<long>? documentIds);
}
