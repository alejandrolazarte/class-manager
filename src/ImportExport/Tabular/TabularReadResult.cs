namespace ClassManager.ImportExport.Tabular;

public sealed class TabularReadResult
{
    private TabularReadResult(TabularData? data, ImportFileError? error)
    {
        Data = data;
        Error = error;
    }

    public TabularData? Data { get; }

    public ImportFileError? Error { get; }

    public static TabularReadResult Success(TabularData data) => new(data, null);

    public static TabularReadResult Failure(ImportFileError error) => new(null, error);
}
