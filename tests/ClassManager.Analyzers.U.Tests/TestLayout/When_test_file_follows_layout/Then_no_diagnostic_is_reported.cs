namespace ClassManager.Analyzers.Tests.TestLayout.When_test_file_follows_layout;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/tests/Api.Tests/When_client_is_registered/Then_returns_201.cs";
    private const string Source = """
        namespace Api.Tests.When_client_is_registered;

        public sealed class Then_returns_201
        {
            [Xunit.Fact]
            public void Then_returns_201_Run() { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
