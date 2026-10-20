namespace ClassManager.Analyzers.Tests.Architecture.When_backoffice_api_uses_the_backoffice;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Backoffice.Api/Endpoints/BrandEndpoints.cs";
    private const string Source = """
        using ClassManager.Backoffice.Core.Brands;

        namespace ClassManager.Backoffice.Api.Endpoints
        {
            public sealed class BrandEndpoints { }
        }

        namespace ClassManager.Backoffice.Core.Brands
        {
            public sealed class BrandSummary { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<BackofficeIsolationAnalyzer>(AnalyzerTestRunner.BackofficeApiAssemblyName, FilePath, Source);
}
