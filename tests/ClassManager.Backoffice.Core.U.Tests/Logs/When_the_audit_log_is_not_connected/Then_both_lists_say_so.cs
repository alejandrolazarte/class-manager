namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_the_audit_log_is_not_connected;

public sealed class Then_both_lists_say_so
{
    [Fact]
    public async Task Then_both_lists_say_so_Run()
    {
        var scenario = new LogsScenario();
        scenario.Reader.IsConnected = false;

        var commands = await scenario.ListCommandsAsync(LogsScenario.Today);
        var events = await scenario.ListSecurityEventsAsync(LogsScenario.Today);

        (commands.Value!.IsConnected, events.Value!.IsConnected).ShouldBe((false, false));
        scenario.Reader.ReadDays.ShouldBeEmpty();
    }
}
