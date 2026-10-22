namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_no_day_is_given;

public sealed class Then_today_is_read
{
    [Fact]
    public async Task Then_today_is_read_Run()
    {
        var scenario = new LogsScenario();

        var result = await scenario.ListCommandsAsync();

        result.Value!.Day.ShouldBe(LogsScenario.Today);
        scenario.Reader.ReadDays.ShouldBe([LogsScenario.Today]);
    }
}
