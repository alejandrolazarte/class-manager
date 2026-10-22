namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_the_commands_of_a_day_are_listed;

public sealed class Then_the_newest_comes_first
{
    [Fact]
    public async Task Then_the_newest_comes_first_Run()
    {
        var scenario = new LogsScenario();
        scenario.WithCommand("RecordPaymentCommand", LogsScenario.Now.AddHours(-3));
        scenario.WithCommand("AddStudentCommand", LogsScenario.Now.AddHours(-1));

        var result = await scenario.ListCommandsAsync(LogsScenario.Today);

        result.Value!.Entries.Select(entry => entry.Command).ShouldBe(["AddStudentCommand", "RecordPaymentCommand"]);
    }
}
