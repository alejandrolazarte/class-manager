using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Domain.Images;

public interface ICatalogImageLink
{
    Guid DocumentId { get; }

    Document Document { get; }

    int Position { get; }

    void MoveTo(int position);
}
