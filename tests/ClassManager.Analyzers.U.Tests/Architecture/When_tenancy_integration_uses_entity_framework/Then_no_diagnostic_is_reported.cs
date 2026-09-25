namespace ClassManager.Analyzers.Tests.Architecture.When_tenancy_integration_uses_entity_framework;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Tenancy.AspNetCore/Persistence/TenantQueryFilters.cs";
    private const string Source = """
        using Microsoft.EntityFrameworkCore;

        namespace ClassManager.Tenancy.AspNetCore.Persistence
        {
            public static class TenantQueryFilters { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<TenancyIndependenceAnalyzer>(AnalyzerTestRunner.TenancyIntegrationAssemblyName, FilePath, Source);
}
