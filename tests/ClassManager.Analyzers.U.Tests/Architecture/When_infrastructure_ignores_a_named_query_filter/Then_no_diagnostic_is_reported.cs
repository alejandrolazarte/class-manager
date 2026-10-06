namespace ClassManager.Analyzers.Tests.Architecture.When_infrastructure_ignores_a_named_query_filter;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Infrastructure/Persistence/PaymentAudit.cs";
    private const string Source = """
        using System.Linq;
        using Microsoft.EntityFrameworkCore;

        namespace Infrastructure.Persistence;

        public static class PaymentAudit
        {
            public static IQueryable<string> ListDeleted(IQueryable<string> payments) => payments.IgnoreQueryFilters(["SoftDelete"]);
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<UnnamedIgnoreQueryFiltersAnalyzer>(AnalyzerTestRunner.InfrastructureAssemblyName, FilePath, Source);
}
