using System.Globalization;
using System.Text;
using ClassManager.ImportExport.Columns;

namespace ClassManager.ImportExport.Parsing;

public static class HeaderMatcher
{
    private static readonly char[] IgnoredCharacters = [' ', '_', '-', '.'];

    public static ColumnMapping Match(IReadOnlyList<string> headers, IReadOnlyList<ImportColumn> columns)
    {
        var columnByName = new Dictionary<string, ImportColumn>(StringComparer.Ordinal);
        foreach (var column in columns)
        {
            foreach (var name in (string[])[column.Key, column.Header, .. column.Aliases])
            {
                columnByName.TryAdd(Normalize(name), column);
            }
        }

        var matchedKeys = new HashSet<string>(StringComparer.Ordinal);
        var mappedHeaders = headers
            .Select(header =>
                columnByName.TryGetValue(Normalize(header), out var column) && matchedKeys.Add(column.Key)
                    ? new MappedHeader(header, column.Key)
                    : new MappedHeader(header, null))
            .ToList();

        var missingRequiredColumns = columns
            .Where(column => column.IsRequired && !matchedKeys.Contains(column.Key))
            .ToList();

        return new ColumnMapping(mappedHeaders, missingRequiredColumns);
    }

    private static string Normalize(string name)
    {
        var decomposedName = name.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var normalizedName = new StringBuilder(decomposedName.Length);
        foreach (var character in decomposedName)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark
                && !IgnoredCharacters.Contains(character))
            {
                normalizedName.Append(character);
            }
        }

        return normalizedName.ToString();
    }
}
