namespace ClassManager.Analyzers.Tests.Architecture.When_api_calls_ignore_query_filters;

public sealed class Then_ARCH002_is_reported
{
    private const string FilePath = "/src/Api/Endpoints/ClientEndpoints.cs";
    private const string Source = """
        using System.Linq;
        using Microsoft.EntityFrameworkCore;

        namespace Api.Endpoints;

        public static class ClientEndpoints
        {
            public static IQueryable<string> ListAll(IQueryable<string> clients) => {|ARCH002:clients.IgnoreQueryFilters()|};
        }
        """;

    [Fact]
    public Task Then_ARCH002_is_reported_Run() => AnalyzerTestRunner.RunAsync<IgnoreQueryFiltersAnalyzer>(AnalyzerTestRunner.ApiAssemblyName, FilePath, Source);
}
