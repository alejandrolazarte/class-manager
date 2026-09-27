using System.Text;

namespace ClassManager.ImportExport.Tabular.Csv;

public sealed class CsvTabularWriter : ITabularWriter
{
    public const char Delimiter = ';';
    public const string CsvContentType = "text/csv";
    public const string CsvFileExtension = ".csv";

    private const char Quote = '"';
    private const string EscapedQuote = "\"\"";
    private const string LineEnding = "\r\n";

    private static readonly UTF8Encoding Utf8WithBom = new(encoderShouldEmitUTF8Identifier: true);
    private static readonly char[] CharactersThatNeedQuotes = [Delimiter, Quote, '\r', '\n'];

    public string ContentType => CsvContentType;

    public string FileExtension => CsvFileExtension;

    public async Task WriteAsync(
        Stream destination,
        IReadOnlyList<string> headers,
        IEnumerable<IReadOnlyList<string?>> rows,
        CancellationToken cancellationToken)
    {
        var writer = new StreamWriter(destination, Utf8WithBom, leaveOpen: true);
        await using (writer.ConfigureAwait(false))
        {
            await WriteRecordAsync(writer, headers, cancellationToken).ConfigureAwait(false);
            foreach (var row in rows)
            {
                await WriteRecordAsync(writer, row, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private static async Task WriteRecordAsync(StreamWriter writer, IReadOnlyList<string?> cells, CancellationToken cancellationToken)
    {
        var record = string.Join(Delimiter, cells.Select(FormatCell)) + LineEnding;
        await writer.WriteAsync(record.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    private static string FormatCell(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var escapedValue = FormulaEscaping.Escape(value);
        return escapedValue.IndexOfAny(CharactersThatNeedQuotes) < 0
            ? escapedValue
            : Quote + escapedValue.Replace(Quote.ToString(), EscapedQuote, StringComparison.Ordinal) + Quote;
    }
}
