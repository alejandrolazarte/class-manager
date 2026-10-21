namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_the_commands_of_a_brand_are_listed;

public sealed class Then_only_its_branches_are_read
{
    [Fact]
    public async Task Then_only_its_branches_are_read_Run()
    {
        var scenario = new LogsScenario();
        var branch = scenario.WithBranch("Estudio Centro", "Sede Norte");
        var otherBranch = scenario.WithBranch("Otra marca", "Sede");
        scenario.WithCommand("RecordPaymentCommand", LogsScenario.Now, branch.BranchId);
        scenario.WithCommand("AddStudentCommand", LogsScenario.Now, otherBranch.BranchId);

        var result = await scenario.ListCommandsAsync(LogsScenario.Today, branch.BrandId);

        result.Value!.Entries.ShouldHaveSingleItem().Command.ShouldBe("RecordPaymentCommand");
        scenario.Reader.ReadTenantIds.ShouldBe([branch.BranchId]);
    }
}
