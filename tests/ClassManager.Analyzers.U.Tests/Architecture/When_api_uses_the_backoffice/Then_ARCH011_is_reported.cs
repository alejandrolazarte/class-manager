namespace ClassManager.Analyzers.Tests.Architecture.When_api_uses_the_backoffice;

public sealed class Then_ARCH011_is_reported
{
    private const string FilePath = "/src/Api/Endpoints/BrandEndpoints.cs";
    private const string Source = """
        {|ARCH011:using ClassManager.Backoffice.Infrastructure.Brands;|}

        namespace ClassManager.Api.Endpoints
        {
            public sealed class BrandEndpoints { }
        }

        namespace ClassManager.Backoffice.Infrastructure.Brands
        {
            public sealed class BrandRepository { }
        }
        """;

    [Fact]
    public Task Then_ARCH011_is_reported_Run() => AnalyzerTestRunner.RunAsync<BackofficeIsolationAnalyzer>(AnalyzerTestRunner.ApiAssemblyName, FilePath, Source);
}
