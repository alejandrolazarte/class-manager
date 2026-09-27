
namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_second_adult_on_a_phone_already_in_the_file;

public sealed class Then_student_joins_that_family
{
    [Fact]
    public async Task Then_student_joins_that_family_Run()
    {
        var builder = new StudentImportBuilder();

        await builder.PlanAndAddAsync("Ana Pérez;11 5555-6666;;;\nLuis Pérez;11 5555-6666;;;\n");

        builder.AddedClients.Single().FullName.ShouldBe("Ana Pérez");
        builder.AddedStudents.Select(student => student.FullName).ShouldBe(["Ana Pérez", "Luis Pérez"]);
    }
}
