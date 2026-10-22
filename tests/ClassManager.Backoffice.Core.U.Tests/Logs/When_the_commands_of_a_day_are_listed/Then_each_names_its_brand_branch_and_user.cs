namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_the_commands_of_a_day_are_listed;

public sealed class Then_each_names_its_brand_branch_and_user
{
    [Fact]
    public async Task Then_each_names_its_brand_branch_and_user_Run()
    {
        var scenario = new LogsScenario();
        var branch = scenario.WithBranch("Estudio Centro", "Sede Norte");
        var userId = TestIds.Next();
        scenario.Directory.UserNames[userId] = "Ana Pérez";
        scenario.WithCommand("RecordPaymentCommand", LogsScenario.Now, branch.BranchId, userId);

        var result = await scenario.ListCommandsAsync(LogsScenario.Today);

        var entry = result.Value!.Entries.ShouldHaveSingleItem();
        (entry.BrandId, entry.BrandName, entry.BranchName, entry.UserName)
            .ShouldBe((branch.BrandId, "Estudio Centro", "Sede Norte", "Ana Pérez"));
    }
}
