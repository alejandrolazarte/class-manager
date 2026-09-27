namespace ClassManager.Analyzers.Tests.Architecture.When_core_uses_import_export;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Core/ImportExport/Students/StudentImportProfile.cs";
    private const string Source = """
        using ClassManager.ImportExport.Columns;

        namespace ClassManager.Core.ImportExport.Students
        {
            public sealed class StudentImportProfile { }
        }

        namespace ClassManager.ImportExport.Columns
        {
            public sealed class ImportColumn { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<ImportExportIndependenceAnalyzer>(AnalyzerTestRunner.CoreAssemblyName, FilePath, Source);
}
