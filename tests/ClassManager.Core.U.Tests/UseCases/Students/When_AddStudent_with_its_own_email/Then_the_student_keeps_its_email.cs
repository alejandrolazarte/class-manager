namespace ClassManager.Core.U.Tests.UseCases.Students.When_AddStudent_with_its_own_email;

public sealed class Then_the_student_keeps_its_email
{
    [Fact]
    public async Task Then_the_student_keeps_its_email_Run()
    {
        var builder = new AddStudentUseCaseBuilder();

        var response = await builder.Build().ExecuteAsync(AddStudentUseCaseBuilder.ValidCommand() with { Email = "tomas@example.com" }, CancellationToken.None);

        response.Value!.Email.ShouldBe("tomas@example.com");
    }
}
