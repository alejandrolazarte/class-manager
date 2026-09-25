namespace ClassManager.Analyzers.Tests.Architecture.When_security_uses_core;

public sealed class Then_ARCH003_is_reported
{
    private const string FilePath = "/src/Security/Tokens/TokenIssuer.cs";
    private const string Source = """
        {|ARCH003:using ClassManager.Core.Domain.Businesses;|}

        namespace ClassManager.Security.Tokens
        {
            public sealed class TokenIssuer { }
        }

        namespace ClassManager.Core.Domain.Businesses
        {
            public sealed class Business { }
        }
        """;

    [Fact]
    public Task Then_ARCH003_is_reported_Run() => AnalyzerTestRunner.RunAsync<SecurityIndependenceAnalyzer>(AnalyzerTestRunner.SecurityAssemblyName, FilePath, Source);
}
