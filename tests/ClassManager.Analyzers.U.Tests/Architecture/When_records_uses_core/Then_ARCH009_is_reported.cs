namespace ClassManager.Analyzers.Tests.Architecture.When_records_uses_core;

public sealed class Then_ARCH009_is_reported
{
    private const string FilePath = "/src/Records/IDeletedOn.cs";
    private const string Source = """
        {|ARCH009:using ClassManager.Core.Domain.Organizations;|}

        namespace ClassManager.Records
        {
            public interface IDeletedOn { }
        }

        namespace ClassManager.Core.Domain.Organizations
        {
            public sealed class Organization { }
        }
        """;

    [Fact]
    public Task Then_ARCH009_is_reported_Run() => AnalyzerTestRunner.RunAsync<RecordsIndependenceAnalyzer>(AnalyzerTestRunner.RecordsAssemblyName, FilePath, Source);
}
