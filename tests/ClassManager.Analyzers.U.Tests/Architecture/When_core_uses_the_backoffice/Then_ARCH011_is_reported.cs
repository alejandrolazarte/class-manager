namespace ClassManager.Analyzers.Tests.Architecture.When_core_uses_the_backoffice;

public sealed class Then_ARCH011_is_reported
{
    private const string FilePath = "/src/Core/UseCases/Brands/ListBrandsUseCase.cs";
    private const string Source = """
        {|ARCH011:using ClassManager.Backoffice.Core.Brands;|}

        namespace ClassManager.Core.UseCases.Brands
        {
            public sealed class ListBrandsUseCase { }
        }

        namespace ClassManager.Backoffice.Core.Brands
        {
            public sealed class BrandSummary { }
        }
        """;

    [Fact]
    public Task Then_ARCH011_is_reported_Run() => AnalyzerTestRunner.RunAsync<BackofficeIsolationAnalyzer>(AnalyzerTestRunner.CoreAssemblyName, FilePath, Source);
}
