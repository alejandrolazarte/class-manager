namespace ClassManager.Analyzers.Tests.Architecture.When_tenancy_uses_core;

public sealed class Then_ARCH004_is_reported
{
    private const string FilePath = "/src/Tenancy/ITenantContext.cs";
    private const string Source = """
        {|ARCH004:using ClassManager.Core.Domain.Businesses;|}

        namespace ClassManager.Tenancy
        {
            public interface ITenantContext { }
        }

        namespace ClassManager.Core.Domain.Businesses
        {
            public sealed class Business { }
        }
        """;

    [Fact]
    public Task Then_ARCH004_is_reported_Run() => AnalyzerTestRunner.RunAsync<TenancyIndependenceAnalyzer>(AnalyzerTestRunner.TenancyAssemblyName, FilePath, Source);
}
