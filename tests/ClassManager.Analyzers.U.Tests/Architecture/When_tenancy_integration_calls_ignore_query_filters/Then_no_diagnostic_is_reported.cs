namespace ClassManager.Analyzers.Tests.Architecture.When_tenancy_integration_calls_ignore_query_filters;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Tenancy.AspNetCore/Persistence/TenantQueryableExtensions.cs";
    private const string Source = """
        using System.Linq;
        using Microsoft.EntityFrameworkCore;

        namespace Tenancy.AspNetCore.Persistence;

        public static class TenantQueryableExtensions
        {
            public static IQueryable<string> IgnoreTenantFilter(this IQueryable<string> query) => query.IgnoreQueryFilters(["Tenant"]);
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<IgnoreQueryFiltersAnalyzer>(AnalyzerTestRunner.TenancyIntegrationAssemblyName, FilePath, Source);
}
