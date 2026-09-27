namespace ClassManager.ImportExport.Xlsx;

internal static class CellReference
{
    private const int LettersInAlphabet = 26;

    public static string ColumnName(int columnIndex)
    {
        var name = string.Empty;
        for (var remaining = columnIndex + 1; remaining > 0; remaining = (remaining - 1) / LettersInAlphabet)
        {
            name = (char)('A' + ((remaining - 1) % LettersInAlphabet)) + name;
        }

        return name;
    }

    public static int? ColumnIndex(string? reference)
    {
        if (string.IsNullOrEmpty(reference))
        {
            return null;
        }

        var columnNumber = 0;
        foreach (var character in reference)
        {
            if (!char.IsAsciiLetter(character))
            {
                break;
            }

            columnNumber = checked((columnNumber * LettersInAlphabet) + (char.ToUpperInvariant(character) - 'A' + 1));
        }

        return columnNumber == 0 ? null : columnNumber - 1;
    }
}
