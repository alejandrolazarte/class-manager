namespace ClassManager.Analyzers.Tests.Architecture.When_infrastructure_calls_ignore_query_filters;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Infrastructure/Persistence/BusinessRepository.cs";
    private const string Source = """
        using System.Linq;
        using Microsoft.EntityFrameworkCore;

        namespace Infrastructure.Persistence;

        public static class BusinessRepository
        {
            public static IQueryable<string> ListAll(IQueryable<string> businesses) => businesses.IgnoreQueryFilters();
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<IgnoreQueryFiltersAnalyzer>(AnalyzerTestRunner.InfrastructureAssemblyName, FilePath, Source);
}
