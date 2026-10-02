namespace ClassManager.Analyzers.Tests.Architecture.When_notifications_delivery_uses_tenancy;

public sealed class Then_ARCH006_is_reported
{
    private const string FilePath = "/src/Notifications.Delivery/Email/SmtpEmailTransport.cs";
    private const string Source = """
        {|ARCH006:using ClassManager.Tenancy;|}

        namespace ClassManager.Notifications.Delivery.Email
        {
            public sealed class SmtpEmailTransport { }
        }

        namespace ClassManager.Tenancy
        {
            public interface ITenantContext { }
        }
        """;

    [Fact]
    public Task Then_ARCH006_is_reported_Run() => AnalyzerTestRunner.RunAsync<NotificationsIndependenceAnalyzer>(AnalyzerTestRunner.NotificationsDeliveryAssemblyName, FilePath, Source);
}
