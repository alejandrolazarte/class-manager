namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class DocumentRepository(AppDbContext context) : IDocumentRepository
{
    public void Add(Document document) => context.Documents.Add(document);

    public void Remove(Document document) => context.Documents.Remove(document);
}
