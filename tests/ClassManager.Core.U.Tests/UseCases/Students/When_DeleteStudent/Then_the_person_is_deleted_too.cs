namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent;

public sealed class Then_the_person_is_deleted_too
{
    [Fact]
    public async Task Then_the_person_is_deleted_too_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();

        await builder.Build().ExecuteAsync(builder.Command(), CancellationToken.None);

        builder.Student.User.IsDeleted.ShouldBeTrue();
    }
}
