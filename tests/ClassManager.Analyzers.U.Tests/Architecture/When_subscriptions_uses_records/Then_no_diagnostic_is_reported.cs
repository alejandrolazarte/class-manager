namespace ClassManager.Analyzers.Tests.Architecture.When_subscriptions_uses_records;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Subscriptions/Subscribers/Subscription.cs";
    private const string Source = """
        using ClassManager.Records;

        namespace ClassManager.Subscriptions.Subscribers
        {
            public sealed class Subscription { }
        }

        namespace ClassManager.Records
        {
            public interface IDeletedOn { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<SubscriptionsIndependenceAnalyzer>(AnalyzerTestRunner.SubscriptionsAssemblyName, FilePath, Source);
}
