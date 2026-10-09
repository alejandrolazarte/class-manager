namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_child_with_their_own_email;

public sealed class Then_the_student_keeps_that_email
{
    [Fact]
    public async Task Then_the_student_keeps_that_email_Run()
    {
        var builder = new StudentImportBuilder();

        await builder.PlanAndAddAsync(
            "Lucas Gómez;11 5555-6666;maria@example.com;lucas@example.com;María Gómez\n",
            "Alumno;Teléfono;Email de contacto;Email alumno;Responsable\n");

        builder.AddedStudents.Single().Email.ShouldBe("lucas@example.com");
        builder.AddedClients.Single().Email.ShouldBe("maria@example.com");
    }
}
