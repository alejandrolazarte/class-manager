namespace ClassManager.Analyzers.Tests.TestLayout.When_test_file_name_does_not_start_with_then;

public sealed class Then_TEST001_is_reported
{
    private const string FilePath = "/tests/Api.Tests/When_client_is_registered/ClientTests.cs";
    private const string Source = """
        namespace Api.Tests.When_client_is_registered;

        public sealed class ClientTests
        {
            [Xunit.Fact]
            public void {|TEST001:ClientTests_Run|}() { }
        }
        """;

    [Fact]
    public Task Then_TEST001_is_reported_Run() => AnalyzerTestRunner.RunAsync<TestLayoutAnalyzer>(AnalyzerTestRunner.TestAssemblyName, FilePath, Source);
}
