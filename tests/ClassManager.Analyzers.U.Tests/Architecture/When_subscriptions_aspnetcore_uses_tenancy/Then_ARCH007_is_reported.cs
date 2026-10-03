namespace ClassManager.Analyzers.Tests.Architecture.When_subscriptions_aspnetcore_uses_tenancy;

public sealed class Then_ARCH007_is_reported
{
    private const string FilePath = "/src/Subscriptions.AspNetCore/Access/FeatureAccess.cs";
    private const string Source = """
        {|ARCH007:using ClassManager.Tenancy;|}

        namespace ClassManager.Subscriptions.AspNetCore.Access
        {
            public sealed class FeatureAccess { }
        }

        namespace ClassManager.Tenancy
        {
            public interface ITenantContext { }
        }
        """;

    [Fact]
    public Task Then_ARCH007_is_reported_Run() => AnalyzerTestRunner.RunAsync<SubscriptionsIndependenceAnalyzer>(AnalyzerTestRunner.SubscriptionsIntegrationAssemblyName, FilePath, Source);
}
