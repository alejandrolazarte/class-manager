using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Abstractions.Storage;

public interface IDocumentStorageService
{
    Task<Document> StoreAsync(
        DocumentOwner owner,
        long ownerId,
        DocumentContent content,
        DocumentVisibility visibility,
        CancellationToken cancellationToken);

    Task DeleteFileAsync(Document document, CancellationToken cancellationToken);

    Uri PublicUrlOf(Document document);

    Uri PrivateUrlOf(Document document);
}
