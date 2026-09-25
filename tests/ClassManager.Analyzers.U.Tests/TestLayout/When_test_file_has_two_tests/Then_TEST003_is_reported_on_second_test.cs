namespace ClassManager.Analyzers.Tests.TestLayout.When_test_file_has_two_tests;

public sealed class Then_TEST003_is_reported_on_second_test
{
    private const string FilePath = "/tests/Api.Tests/When_client_is_registered/Then_returns_201.cs";
    private const string Source = """
        namespace Api.Tests.When_client_is_registered;

        public sealed class Then_returns_201
        {
            [Xunit.Fact]
            public void Then_returns_201_Run() { }

            [Xunit.Theory]
            public void {|TEST003:RunAgain|}() { }
        }
        """;

    [Fact]
    public Task Then_TEST003_is_reported_on_second_test_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
