namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_the_brand_has_no_branches;

public sealed class Then_no_command_is_read
{
    [Fact]
    public async Task Then_no_command_is_read_Run()
    {
        var scenario = new LogsScenario();
        scenario.WithCommand("RecordPaymentCommand", LogsScenario.Now);

        var result = await scenario.ListCommandsAsync(LogsScenario.Today, TestIds.Next());

        result.Value!.Entries.ShouldBeEmpty();
        scenario.Reader.ReadDays.ShouldBeEmpty();
    }
}
