namespace ClassManager.Core.UseCases.ImportExport;

public static class ImportUseCaseErrorCodes
{
    public const string ModuleNotFound = "import.module_not_found";
    public const string DuplicateInFile = "import.duplicate_in_file";
    public const string ConcurrentChange = "import.concurrent_change";
    public const string FileFieldName = "file";
}
