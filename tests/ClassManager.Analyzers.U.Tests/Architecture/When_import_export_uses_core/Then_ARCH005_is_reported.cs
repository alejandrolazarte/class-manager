namespace ClassManager.Analyzers.Tests.Architecture.When_import_export_uses_core;

public sealed class Then_ARCH005_is_reported
{
    private const string FilePath = "/src/ImportExport/Parsing/ImportParser.cs";
    private const string Source = """
        {|ARCH005:using ClassManager.Core.Domain.Students;|}

        namespace ClassManager.ImportExport.Parsing
        {
            public sealed class ImportParser { }
        }

        namespace ClassManager.Core.Domain.Students
        {
            public sealed class Student { }
        }
        """;

    [Fact]
    public Task Then_ARCH005_is_reported_Run() => AnalyzerTestRunner.RunAsync<ImportExportIndependenceAnalyzer>(AnalyzerTestRunner.ImportExportAssemblyName, FilePath, Source);
}
