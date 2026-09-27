namespace ClassManager.ImportExport.Parsing;

public sealed class ImportRow(
    int lineNumber,
    IReadOnlyDictionary<string, string> textByKey,
    IReadOnlyDictionary<string, DateOnly> dateByKey,
    IReadOnlyList<ImportCellError> errors)
{
    public int LineNumber => lineNumber;

    public IReadOnlyList<ImportCellError> Errors => errors;

    public bool HasErrors => errors.Count > 0;

    public string? GetText(string key) => textByKey.GetValueOrDefault(key);

    public DateOnly? GetDate(string key) => dateByKey.TryGetValue(key, out var date) ? date : null;
}
