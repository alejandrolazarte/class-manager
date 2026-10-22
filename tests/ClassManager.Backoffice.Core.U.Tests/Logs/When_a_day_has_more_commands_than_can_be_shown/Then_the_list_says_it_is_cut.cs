namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_a_day_has_more_commands_than_can_be_shown;

public sealed class Then_the_list_says_it_is_cut
{
    [Fact]
    public async Task Then_the_list_says_it_is_cut_Run()
    {
        var scenario = new LogsScenario();
        scenario.WithCommand("RecordPaymentCommand", LogsScenario.Now);
        scenario.Reader.IsTruncated = true;

        var result = await scenario.ListCommandsAsync(LogsScenario.Today);

        result.Value!.IsTruncated.ShouldBeTrue();
    }
}
