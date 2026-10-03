namespace ClassManager.Analyzers.Tests.Architecture.When_core_uses_subscriptions;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Core/Domain/Subscriptions/Features.cs";
    private const string Source = """
        using ClassManager.Subscriptions.Access;

        namespace ClassManager.Core.Domain.Subscriptions
        {
            public static class Features { }
        }

        namespace ClassManager.Subscriptions.Access
        {
            public interface IFeatureAccess { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<SubscriptionsIndependenceAnalyzer>(AnalyzerTestRunner.CoreAssemblyName, FilePath, Source);
}
