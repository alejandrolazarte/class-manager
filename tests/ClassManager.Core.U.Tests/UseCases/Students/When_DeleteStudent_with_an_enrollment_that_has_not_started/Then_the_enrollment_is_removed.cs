namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent_with_an_enrollment_that_has_not_started;

public sealed class Then_the_enrollment_is_removed
{
    [Fact]
    public async Task Then_the_enrollment_is_removed_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();
        var enrollment = builder.WithEnrollmentStartingOn(TestData.Today.AddDays(7));

        await builder.Build().ExecuteAsync(builder.Command(), CancellationToken.None);

        builder.Enrollments.Verify(repository => repository.Remove(enrollment), Times.Once);
    }
}
