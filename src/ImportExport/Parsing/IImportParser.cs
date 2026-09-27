using ClassManager.ImportExport.Columns;

namespace ClassManager.ImportExport.Parsing;

public interface IImportParser
{
    Task<ImportParseResult> ParseAsync(
        Stream file,
        IReadOnlyList<ImportColumn> columns,
        ImportLimits limits,
        CancellationToken cancellationToken);
}
