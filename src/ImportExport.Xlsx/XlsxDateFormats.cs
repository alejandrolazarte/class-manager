using System.Text.RegularExpressions;

namespace ClassManager.ImportExport.Xlsx;

internal static partial class XlsxDateFormats
{
    private static readonly HashSet<int> BuiltInDateFormatIds = [14, 15, 16, 17, 22, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 50, 51, 52, 53, 54, 55, 56, 57, 58];

    public static bool IsDateFormat(int numberFormatId, IReadOnlyDictionary<int, string> customFormatCodes) =>
        BuiltInDateFormatIds.Contains(numberFormatId)
        || (customFormatCodes.TryGetValue(numberFormatId, out var formatCode) && HasDateParts(formatCode));

    private static bool HasDateParts(string formatCode)
    {
        var withoutLiterals = LiteralsPattern().Replace(formatCode, string.Empty);
        return withoutLiterals.Contains('d', StringComparison.OrdinalIgnoreCase)
            || withoutLiterals.Contains('y', StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex("""\"[^\"]*\"|\[[^\]]*\]|\\.""")]
    private static partial Regex LiteralsPattern();
}
