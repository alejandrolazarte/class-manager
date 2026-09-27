using System.Globalization;
using ClassManager.ImportExport.Columns;
using ClassManager.ImportExport.Tabular;

namespace ClassManager.ImportExport.Parsing;

public sealed class ImportParser(ITabularReader tabularReader) : IImportParser
{
    private const int ReadBufferSize = 81920;
    private const string HeaderSeparator = ", ";
    private const string FileTooLargeMessage = "The file is larger than {0} KB.";
    private const string TooManyRowsMessage = "The file has more than {0} rows.";
    private const string MissingColumnsMessage = "Required columns are missing: {0}.";
    private const string RequiredMessage = "{0} is required.";
    private const string InvalidDateMessage = "{0} must be a date such as 31/12/2015 or 2015-12-31.";
    private const int BytesPerKilobyte = 1024;

    private static readonly string[] DateFormats = ["d/M/yyyy", "d-M-yyyy", "yyyy-M-d"];

    public async Task<ImportParseResult> ParseAsync(
        Stream file,
        IReadOnlyList<ImportColumn> columns,
        ImportLimits limits,
        CancellationToken cancellationToken)
    {
        var content = await ReadWithinLimitAsync(file, limits.MaximumFileSizeInBytes, cancellationToken).ConfigureAwait(false);
        if (content is null)
        {
            return Failure(ImportErrorCodes.FileTooLarge, FileTooLargeMessage, limits.MaximumFileSizeInBytes / BytesPerKilobyte);
        }

        var readResult = tabularReader.Read(content);
        if (readResult.Error is not null)
        {
            return ImportParseResult.Failure(readResult.Error);
        }

        var data = readResult.Data!;
        if (data.Rows.Count > limits.MaximumRowCount)
        {
            return Failure(ImportErrorCodes.TooManyRows, TooManyRowsMessage, limits.MaximumRowCount);
        }

        var mapping = HeaderMatcher.Match(data.Headers, columns);
        if (mapping.MissingRequiredColumns.Count > 0)
        {
            var missingHeaders = string.Join(HeaderSeparator, mapping.MissingRequiredColumns.Select(column => column.Header));
            return Failure(ImportErrorCodes.MissingColumns, MissingColumnsMessage, missingHeaders);
        }

        var rows = data.Rows.Select(row => ToImportRow(row, columns, mapping)).ToList();
        return ImportParseResult.Success(new ParsedImport(mapping, rows));
    }

    private static async Task<byte[]?> ReadWithinLimitAsync(Stream file, int maximumSizeInBytes, CancellationToken cancellationToken)
    {
        using var content = new MemoryStream();
        var buffer = new byte[ReadBufferSize];
        int bytesRead;
        while ((bytesRead = await file.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) > 0)
        {
            if (content.Length + bytesRead > maximumSizeInBytes)
            {
                return null;
            }

            content.Write(buffer, 0, bytesRead);
        }

        return content.ToArray();
    }

    private static ImportRow ToImportRow(TabularRow row, IReadOnlyList<ImportColumn> columns, ColumnMapping mapping)
    {
        var textByKey = new Dictionary<string, string>(StringComparer.Ordinal);
        var dateByKey = new Dictionary<string, DateOnly>(StringComparer.Ordinal);
        var errors = new List<ImportCellError>();

        foreach (var column in columns)
        {
            var text = CellText(row, mapping.IndexOf(column.Key));
            if (text is null)
            {
                if (column.IsRequired)
                {
                    errors.Add(CellError(column, ImportErrorCodes.Required, RequiredMessage));
                }

                continue;
            }

            textByKey[column.Key] = text;
            if (column.Type != ColumnType.Date)
            {
                continue;
            }

            if (DateOnly.TryParseExact(text, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                dateByKey[column.Key] = date;
            }
            else
            {
                errors.Add(CellError(column, ImportErrorCodes.InvalidDate, InvalidDateMessage));
            }
        }

        return new ImportRow(row.LineNumber, textByKey, dateByKey, errors);
    }

    private static string? CellText(TabularRow row, int? index)
    {
        if (index is not { } cellIndex || cellIndex >= row.Cells.Count)
        {
            return null;
        }

        var cell = row.Cells[cellIndex];
        return string.IsNullOrWhiteSpace(cell) ? null : FormulaEscaping.Unescape(cell.Trim());
    }

    private static ImportCellError CellError(ImportColumn column, string code, string messageFormat) =>
        new(column.Key, code, string.Format(CultureInfo.InvariantCulture, messageFormat, column.Header));

    private static ImportParseResult Failure(string code, string messageFormat, object argument) =>
        ImportParseResult.Failure(new ImportFileError(code, string.Format(CultureInfo.InvariantCulture, messageFormat, argument)));
}
