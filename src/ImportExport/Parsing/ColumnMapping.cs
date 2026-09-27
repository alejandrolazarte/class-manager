using ClassManager.ImportExport.Columns;

namespace ClassManager.ImportExport.Parsing;

public sealed record ColumnMapping(
    IReadOnlyList<MappedHeader> Headers,
    IReadOnlyList<ImportColumn> MissingRequiredColumns)
{
    public int? IndexOf(string key)
    {
        for (var index = 0; index < Headers.Count; index++)
        {
            if (Headers[index].Key == key)
            {
                return index;
            }
        }

        return null;
    }
}
