namespace ClassManager.Analyzers.Tests.TestLayout.When_namespace_does_not_end_with_when_folder;

public sealed class Then_TEST004_is_reported
{
    private const string FilePath = "/tests/Api.Tests/When_client_is_registered/Then_returns_201.cs";
    private const string Source = """
        namespace Api.Tests;

        public sealed class Then_returns_201
        {
            [Xunit.Fact]
            public void {|TEST004:Then_returns_201_Run|}() { }
        }
        """;

    [Fact]
    public Task Then_TEST004_is_reported_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
