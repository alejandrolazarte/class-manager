using ClassManager.Core.Domain.Documents;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IDocumentRepository
{
    void Add(Document document);

    void Remove(Document document);
}
