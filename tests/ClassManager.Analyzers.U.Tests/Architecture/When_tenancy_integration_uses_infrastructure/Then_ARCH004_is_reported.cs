namespace ClassManager.Analyzers.Tests.Architecture.When_tenancy_integration_uses_infrastructure;

public sealed class Then_ARCH004_is_reported
{
    private const string FilePath = "/src/Tenancy.AspNetCore/Persistence/TenantQueryFilters.cs";
    private const string Source = """
        {|ARCH004:using ClassManager.Infrastructure.Persistence;|}

        namespace ClassManager.Tenancy.AspNetCore.Persistence
        {
            public static class TenantQueryFilters { }
        }

        namespace ClassManager.Infrastructure.Persistence
        {
            public sealed class AppDbContext { }
        }
        """;

    [Fact]
    public Task Then_ARCH004_is_reported_Run() => AnalyzerTestRunner.RunAsync<TenancyIndependenceAnalyzer>(AnalyzerTestRunner.TenancyIntegrationAssemblyName, FilePath, Source);
}
