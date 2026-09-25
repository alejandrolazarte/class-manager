namespace ClassManager.Analyzers.Tests.Architecture.When_infrastructure_uses_core;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Infrastructure/Security/TokenService.cs";
    private const string Source = """
        using ClassManager.Core.Domain.Businesses;

        namespace ClassManager.Infrastructure.Security
        {
            public sealed class TokenService { }
        }

        namespace ClassManager.Core.Domain.Businesses
        {
            public sealed class Business { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<SecurityIndependenceAnalyzer>(AnalyzerTestRunner.InfrastructureAssemblyName, FilePath, Source);
}
