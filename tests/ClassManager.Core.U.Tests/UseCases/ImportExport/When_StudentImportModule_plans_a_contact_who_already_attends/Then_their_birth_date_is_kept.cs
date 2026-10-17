namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_contact_who_already_attends;

public sealed class Then_their_birth_date_is_kept
{
    [Fact]
    public async Task Then_their_birth_date_is_kept_Run()
    {
        var builder = new StudentImportBuilder();
        var client = builder.AddExistingClient(TestData.ClientFullName);
        client.User.ChangeBirthDate(new DateOnly(1990, 5, 10), TestData.Today);

        await builder.PlanAndAddAsync("Ana Pérez;11 2233 4455;;07/03/1985;\n");

        client.User.BirthDate.ShouldBe(new DateOnly(1990, 5, 10));
    }
}
