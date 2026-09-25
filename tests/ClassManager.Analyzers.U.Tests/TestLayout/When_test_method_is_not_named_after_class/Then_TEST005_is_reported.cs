namespace ClassManager.Analyzers.Tests.TestLayout.When_test_method_is_not_named_after_class;

public sealed class Then_TEST005_is_reported
{
    private const string FilePath = "/tests/Api.Tests/When_client_is_registered/Then_returns_201.cs";
    private const string Source = """
        namespace Api.Tests.When_client_is_registered;

        public sealed class Then_returns_201
        {
            [Xunit.Fact]
            public void {|TEST005:Returns_created|}() { }
        }
        """;

    [Fact]
    public Task Then_TEST005_is_reported_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
