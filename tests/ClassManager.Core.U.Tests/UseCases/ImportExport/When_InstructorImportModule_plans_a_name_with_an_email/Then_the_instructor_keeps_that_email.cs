namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_a_name_with_an_email;

public sealed class Then_the_instructor_keeps_that_email
{
    [Fact]
    public async Task Then_the_instructor_keeps_that_email_Run()
    {
        var builder = new ImportUseCaseBuilder();

        var plan = await builder.PlanInstructorsAsync("Profesor;Email\nMarta Ruiz;marta@example.com\n");
        plan.AddValidRows();

        builder.AddedInstructors.Single().User.Email.ShouldBe("marta@example.com");
    }
}
