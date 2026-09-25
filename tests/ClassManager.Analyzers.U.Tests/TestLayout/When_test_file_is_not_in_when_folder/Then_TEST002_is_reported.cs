namespace ClassManager.Analyzers.Tests.TestLayout.When_test_file_is_not_in_when_folder;

public sealed class Then_TEST002_is_reported
{
    private const string FilePath = "/tests/Api.Tests/Clients/Then_returns_201.cs";
    private const string Source = """
        namespace Api.Tests.Clients;

        public sealed class Then_returns_201
        {
            [Xunit.Fact]
            public void {|TEST002:Then_returns_201_Run|}() { }
        }
        """;

    [Fact]
    public Task Then_TEST002_is_reported_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
