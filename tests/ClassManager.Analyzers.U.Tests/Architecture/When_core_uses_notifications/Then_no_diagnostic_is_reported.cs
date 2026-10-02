namespace ClassManager.Analyzers.Tests.Architecture.When_core_uses_notifications;

public sealed class Then_no_diagnostic_is_reported
{
    private const string FilePath = "/src/Core/Abstractions/Email/EmailMessage.cs";
    private const string Source = """
        using ClassManager.Notifications.Email;

        namespace ClassManager.Core.Abstractions.Email
        {
            public sealed class EmailMessage { }
        }

        namespace ClassManager.Notifications.Email
        {
            public sealed class EmailContent { }
        }
        """;

    [Fact]
    public Task Then_no_diagnostic_is_reported_Run() => AnalyzerTestRunner.RunAsync<NotificationsIndependenceAnalyzer>(AnalyzerTestRunner.CoreAssemblyName, FilePath, Source);
}
