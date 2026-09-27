
namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_an_invalid_student_before_its_sibling;

public sealed class Then_sibling_still_creates_the_family
{
    [Fact]
    public async Task Then_sibling_still_creates_the_family_Run()
    {
        var builder = new StudentImportBuilder();

        await builder.PlanAndAddAsync("L;11 5555-6666;;;María Gómez\nSofía Gómez;1155556666;;;María Gómez\n");

        builder.AddedClients.Single().FullName.ShouldBe("María Gómez");
        builder.AddedStudents.Single().FullName.ShouldBe("Sofía Gómez");
    }
}
