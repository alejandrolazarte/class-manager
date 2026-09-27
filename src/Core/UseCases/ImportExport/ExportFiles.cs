using ClassManager.ImportExport.Columns;
using ClassManager.ImportExport.Tabular;

namespace ClassManager.Core.UseCases.ImportExport;

internal static class ExportFiles
{
    public static async Task<ExportFile> WriteAsync(
        ITabularWriter writer,
        string fileNameWithoutExtension,
        IReadOnlyList<ImportColumn> columns,
        IEnumerable<IReadOnlyDictionary<string, string?>> rows,
        CancellationToken cancellationToken)
    {
        using var content = new MemoryStream();
        await writer.WriteAsync(
            content,
            columns.Select(column => column.Header).ToList(),
            rows.Select(row => columns.Select(column => row.GetValueOrDefault(column.Key)).ToList()),
            cancellationToken);

        return new ExportFile(fileNameWithoutExtension + writer.FileExtension, writer.ContentType, content.ToArray());
    }
}
