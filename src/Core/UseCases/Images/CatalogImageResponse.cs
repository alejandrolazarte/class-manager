using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.UseCases.Images;

public sealed record CatalogImageResponse(Guid Id, string Url)
{
    public static IReadOnlyList<CatalogImageResponse> ListFrom(IReadOnlyList<Document> documents, IDocumentStorageService documentStorage) =>
        [.. documents.Select(document => new CatalogImageResponse(document.Id, documentStorage.PublicUrlOf(document).AbsoluteUri))];
}
