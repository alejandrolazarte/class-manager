namespace ClassManager.Core.U.Tests.UseCases.Students.When_UpdateStudent_keeps_the_email_used_to_sign_in;

public sealed class Then_the_student_is_saved
{
    [Fact]
    public async Task Then_the_student_is_saved_Run()
    {
        var builder = new UpdateStudentUseCaseBuilder();
        builder.WithStudentAppAccount();

        var response = await builder.Build().ExecuteAsync(
            builder.ValidCommand() with { Email = UpdateStudentUseCaseBuilder.SignInEmail },
            CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
    }
}
