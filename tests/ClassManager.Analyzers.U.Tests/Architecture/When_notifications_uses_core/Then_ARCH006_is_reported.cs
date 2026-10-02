namespace ClassManager.Analyzers.Tests.Architecture.When_notifications_uses_core;

public sealed class Then_ARCH006_is_reported
{
    private const string FilePath = "/src/Notifications/Email/BrandedEmailHtml.cs";
    private const string Source = """
        {|ARCH006:using ClassManager.Core.Domain.Businesses;|}

        namespace ClassManager.Notifications.Email
        {
            public static class BrandedEmailHtml { }
        }

        namespace ClassManager.Core.Domain.Businesses
        {
            public sealed class Business { }
        }
        """;

    [Fact]
    public Task Then_ARCH006_is_reported_Run() => AnalyzerTestRunner.RunAsync<NotificationsIndependenceAnalyzer>(AnalyzerTestRunner.NotificationsAssemblyName, FilePath, Source);
}
