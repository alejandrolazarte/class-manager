namespace ClassManager.ImportExport.Parsing;

public sealed class ImportParseResult
{
    private ImportParseResult(ParsedImport? import, ImportFileError? error)
    {
        Import = import;
        Error = error;
    }

    public ParsedImport? Import { get; }

    public ImportFileError? Error { get; }

    public static ImportParseResult Success(ParsedImport import) => new(import, null);

    public static ImportParseResult Failure(ImportFileError error) => new(null, error);
}
