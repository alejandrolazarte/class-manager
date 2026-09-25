namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_duplicate_student_names;

public sealed class Then_field_error_names_second_student
{
    [Fact]
    public async Task Then_field_error_names_second_student_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.CommandWithStudents("Tomás Pérez", " tomás pérez ");

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.FieldName.ShouldBe("Students[1].FullName");
    }
}
