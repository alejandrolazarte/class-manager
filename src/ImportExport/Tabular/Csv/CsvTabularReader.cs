using System.Text;

namespace ClassManager.ImportExport.Tabular.Csv;

public sealed class CsvTabularReader : ITabularReader
{
    private const char Quote = '"';
    private const char LineFeed = '\n';
    private const char DefaultDelimiter = ',';
    private const int WesternEuropeanWindowsCodePage = 1252;
    private const string UnreadableFileMessage = "The file could not be read as CSV: a quoted cell is not closed.";
    private const string EmptyFileMessage = "The file is empty: the first row must contain the column names.";

    private static readonly char[] CandidateDelimiters = [';', ',', '\t'];
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly Encoding WesternEuropeanWindows = CodePagesEncodingProvider.Instance.GetEncoding(WesternEuropeanWindowsCodePage)!;

    public TabularReadResult Read(ReadOnlyMemory<byte> content)
    {
        var text = Decode(content.Span);
        var records = new CsvRecordParser(text, DetectDelimiter(text)).Parse();
        if (records is null)
        {
            return TabularReadResult.Failure(new ImportFileError(ImportErrorCodes.UnreadableFile, UnreadableFileMessage));
        }

        var nonBlankRecords = records.Where(record => !record.Cells.All(string.IsNullOrWhiteSpace)).ToList();
        if (nonBlankRecords.Count == 0)
        {
            return TabularReadResult.Failure(new ImportFileError(ImportErrorCodes.EmptyFile, EmptyFileMessage));
        }

        var headers = nonBlankRecords[0].Cells.Select(header => header.Trim()).ToList();
        return TabularReadResult.Success(new TabularData(headers, nonBlankRecords[1..]));
    }

    private static string Decode(ReadOnlySpan<byte> content)
    {
        var utf8Preamble = Encoding.UTF8.Preamble;
        if (content.StartsWith(utf8Preamble))
        {
            content = content[utf8Preamble.Length..];
        }

        try
        {
            return StrictUtf8.GetString(content);
        }
        catch (DecoderFallbackException)
        {
            return WesternEuropeanWindows.GetString(content);
        }
    }

    private static char DetectDelimiter(string text)
    {
        var firstLine = text.Split(LineFeed).FirstOrDefault(line => !string.IsNullOrWhiteSpace(line)) ?? string.Empty;

        var bestDelimiter = DefaultDelimiter;
        var bestCount = 0;
        foreach (var candidate in CandidateDelimiters)
        {
            var count = CountOutsideQuotes(firstLine, candidate);
            if (count > bestCount)
            {
                bestDelimiter = candidate;
                bestCount = count;
            }
        }

        return bestDelimiter;
    }

    private static int CountOutsideQuotes(string line, char candidate)
    {
        var count = 0;
        var isInsideQuotes = false;
        foreach (var character in line)
        {
            if (character == Quote)
            {
                isInsideQuotes = !isInsideQuotes;
            }
            else if (character == candidate && !isInsideQuotes)
            {
                count++;
            }
        }

        return count;
    }
}
