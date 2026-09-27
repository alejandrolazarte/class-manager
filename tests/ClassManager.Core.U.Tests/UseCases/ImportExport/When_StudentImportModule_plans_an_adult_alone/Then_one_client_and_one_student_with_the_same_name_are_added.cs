
namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_an_adult_alone;

public sealed class Then_one_client_and_one_student_with_the_same_name_are_added
{
    [Fact]
    public async Task Then_one_client_and_one_student_with_the_same_name_are_added_Run()
    {
        var builder = new StudentImportBuilder();

        await builder.PlanAndAddAsync("Ana Pérez;11 5555-6666;;;\n");

        builder.AddedClients.Single().FullName.ShouldBe("Ana Pérez");
        builder.AddedStudents.Single().FullName.ShouldBe("Ana Pérez");
        builder.AddedStudents.Single().ClientId.ShouldBe(builder.AddedClients.Single().Id);
    }
}
