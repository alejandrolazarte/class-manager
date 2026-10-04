
namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_siblings_with_the_same_phone;

public sealed class Then_one_client_with_two_students_is_added
{
    [Fact]
    public async Task Then_one_client_with_two_students_is_added_Run()
    {
        var builder = new StudentImportBuilder();

        await builder.PlanAndAddAsync("Lucas Gómez;11 5555-6666;;;María Gómez\nSofía Gómez;1155556666;;;María Gómez\n");

        builder.AddedClients.Single().FullName.ShouldBe("María Gómez");
        builder.AddedStudents.Select(student => student.ClientId).Distinct().ShouldBe([builder.AddedClients.Single().Id]);
        builder.AddedStudents.Count.ShouldBe(2);
    }
}
