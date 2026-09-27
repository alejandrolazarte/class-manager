namespace ClassManager.ImportExport.Tabular;

public interface ITabularWriter
{
    string ContentType { get; }

    string FileExtension { get; }

    Task WriteAsync(
        Stream destination,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<string?>> rows,
        CancellationToken cancellationToken);
}
