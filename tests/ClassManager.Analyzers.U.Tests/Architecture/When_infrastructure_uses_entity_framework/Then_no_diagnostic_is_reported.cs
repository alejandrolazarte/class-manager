namespace ClassManager.Analyzers.Tests.Architecture.When_infrastructure_uses_entity_framework;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Infrastructure/Persistence/ClientRepository.cs";
    private const string Source = """
        using Microsoft.EntityFrameworkCore;

        namespace Infrastructure.Persistence;

        public sealed class ClientRepository { }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<CoreDependencyAnalyzer>(AnalyzerTestRunner.InfrastructureAssemblyName, FilePath, Source);
}
