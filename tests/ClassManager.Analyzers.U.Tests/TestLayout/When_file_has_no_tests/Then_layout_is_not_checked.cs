namespace ClassManager.Analyzers.Tests.TestLayout.When_file_has_no_tests;

public sealed class Then_layout_is_not_checked
{
    private const string FilePath = "/tests/Api.Tests/SqlServerContainerFixture.cs";
    private const string Source = """
        namespace Api.Tests;

        public sealed class SqlServerContainerFixture
        {
            public void Start() { }
        }
        """;

    [Fact]
    public Task Then_layout_is_not_checked_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
