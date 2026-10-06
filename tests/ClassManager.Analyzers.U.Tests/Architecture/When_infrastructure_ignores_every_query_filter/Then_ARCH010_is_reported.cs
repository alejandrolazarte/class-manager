namespace ClassManager.Analyzers.Tests.Architecture.When_infrastructure_ignores_every_query_filter;

public sealed class Then_ARCH010_is_reported
{
    private const string FilePath = "/src/Infrastructure/Security/BranchDirectory.cs";
    private const string Source = """
        using System.Linq;
        using Microsoft.EntityFrameworkCore;

        namespace Infrastructure.Security;

        public static class BranchDirectory
        {
            public static IQueryable<string> ListAll(IQueryable<string> members) => {|ARCH010:members.IgnoreQueryFilters()|};
        }
        """;

    [Fact]
    public Task Then_ARCH010_is_reported_Run() => AnalyzerTestRunner.RunAsync<UnnamedIgnoreQueryFiltersAnalyzer>(AnalyzerTestRunner.InfrastructureAssemblyName, FilePath, Source);
}
