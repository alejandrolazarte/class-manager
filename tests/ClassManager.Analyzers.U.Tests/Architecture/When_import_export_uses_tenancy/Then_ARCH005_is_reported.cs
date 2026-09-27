namespace ClassManager.Analyzers.Tests.Architecture.When_import_export_uses_tenancy;

public sealed class Then_ARCH005_is_reported
{
    private const string FilePath = "/src/ImportExport/Parsing/ImportParser.cs";
    private const string Source = """
        {|ARCH005:using ClassManager.Tenancy;|}

        namespace ClassManager.ImportExport.Parsing
        {
            public sealed class ImportParser { }
        }

        namespace ClassManager.Tenancy
        {
            public interface ITenantContext { }
        }
        """;

    [Fact]
    public Task Then_ARCH005_is_reported_Run() => AnalyzerTestRunner.RunAsync<ImportExportIndependenceAnalyzer>(AnalyzerTestRunner.ImportExportAssemblyName, FilePath, Source);
}
