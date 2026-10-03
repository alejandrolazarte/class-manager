namespace ClassManager.Analyzers.Tests.Architecture.When_subscriptions_uses_core;

public sealed class Then_ARCH007_is_reported
{
    private const string FilePath = "/src/Subscriptions/Access/EffectiveFeatures.cs";
    private const string Source = """
        {|ARCH007:using ClassManager.Core.Domain.Organizations;|}

        namespace ClassManager.Subscriptions.Access
        {
            public sealed class EffectiveFeatures { }
        }

        namespace ClassManager.Core.Domain.Organizations
        {
            public sealed class Organization { }
        }
        """;

    [Fact]
    public Task Then_ARCH007_is_reported_Run() => AnalyzerTestRunner.RunAsync<SubscriptionsIndependenceAnalyzer>(AnalyzerTestRunner.SubscriptionsAssemblyName, FilePath, Source);
}
