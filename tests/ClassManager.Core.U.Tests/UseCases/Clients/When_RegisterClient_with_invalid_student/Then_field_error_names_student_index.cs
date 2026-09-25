namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_with_invalid_student;

public sealed class Then_field_error_names_student_index
{
    [Fact]
    public async Task Then_field_error_names_student_index_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.CommandWithStudents("Tomás Pérez", "L");

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        response.Error!.FieldName.ShouldBe("Students[1].FullName");
    }
}
