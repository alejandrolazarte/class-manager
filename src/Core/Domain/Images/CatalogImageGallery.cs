using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Domain.Images;

public static class CatalogImageGallery
{
    public const string DocumentIdsFieldName = "DocumentIds";

    private const string OrderMismatchMessage = "The new order must list every photo exactly once.";

    public static IReadOnlyList<Document> InOrder<TLink>(IEnumerable<TLink> links)
        where TLink : ICatalogImageLink =>
        [.. links.OrderBy(link => link.Position).Select(link => link.Document)];

    public static Document? Remove<TLink>(List<TLink> links, Guid documentId)
        where TLink : ICatalogImageLink
    {
        var removedLink = links.FirstOrDefault(link => link.DocumentId == documentId);
        if (removedLink is null)
        {
            return null;
        }

        links.Remove(removedLink);
        Renumber(links.OrderBy(link => link.Position));
        return removedLink.Document;
    }

    public static Result Reorder<TLink>(List<TLink> links, IReadOnlyList<Guid>? documentIds)
        where TLink : ICatalogImageLink
    {
        if (documentIds is null
            || documentIds.Count != links.Count
            || documentIds.Distinct().Count() != documentIds.Count
            || documentIds.Any(documentId => links.All(link => link.DocumentId != documentId)))
        {
            return Result.Validation(OrderMismatchMessage, ImageErrorCodes.OrderMismatch, DocumentIdsFieldName);
        }

        Renumber(documentIds.Select(documentId => links.Single(link => link.DocumentId == documentId)));
        return Result.Success();
    }

    private static void Renumber<TLink>(IEnumerable<TLink> orderedLinks)
        where TLink : ICatalogImageLink
    {
        var position = 0;
        foreach (var link in orderedLinks.ToList())
        {
            link.MoveTo(position++);
        }
    }
}
