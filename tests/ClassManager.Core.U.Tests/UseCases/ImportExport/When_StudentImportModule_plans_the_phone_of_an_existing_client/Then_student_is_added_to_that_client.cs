
namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_the_phone_of_an_existing_client;

public sealed class Then_student_is_added_to_that_client
{
    [Fact]
    public async Task Then_student_is_added_to_that_client_Run()
    {
        var builder = new StudentImportBuilder();
        var existingClient = builder.AddExistingClient();

        await builder.PlanAndAddAsync("Tomás Pérez;1122334455;;;\n");

        builder.AddedClients.ShouldBeEmpty();
        builder.AddedStudents.Single().ClientId.ShouldBe(existingClient.Id);
    }
}
