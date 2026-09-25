namespace ClassManager.Analyzers.Tests.Architecture.When_core_uses_entity_framework;

public sealed class Then_ARCH001_is_reported
{
    private const string FilePath = "/src/Core/Domain/Clients/Client.cs";
    private const string Source = """
        {|ARCH001:using Microsoft.EntityFrameworkCore;|}

        namespace Core.Domain.Clients;

        public sealed class Client { }
        """;

    [Fact]
    public Task Then_ARCH001_is_reported_Run() => AnalyzerTestRunner.RunAsync<CoreDependencyAnalyzer>(AnalyzerTestRunner.CoreAssemblyName, FilePath, Source);
}
