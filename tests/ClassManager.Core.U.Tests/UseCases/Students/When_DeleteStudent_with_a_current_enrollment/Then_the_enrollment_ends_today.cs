namespace ClassManager.Core.U.Tests.UseCases.Students.When_DeleteStudent_with_a_current_enrollment;

public sealed class Then_the_enrollment_ends_today
{
    [Fact]
    public async Task Then_the_enrollment_ends_today_Run()
    {
        var builder = new DeleteStudentUseCaseBuilder();
        var enrollment = builder.WithEnrollmentStartingOn(TestData.Today.AddMonths(-2));

        var response = await builder.Build().ExecuteAsync(builder.Command(), CancellationToken.None);

        enrollment.EndDate.ShouldBe(TestData.Today);
        response.Value!.EndedEnrollments.ShouldBe(1);
    }
}
