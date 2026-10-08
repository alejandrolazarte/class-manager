namespace ClassManager.Core.U.Tests.UseCases.Enrollments.When_EnrollStudent_who_leaves_today;

public sealed class Then_the_enrollment_continues
{
    [Fact]
    public async Task Then_the_enrollment_continues_Run()
    {
        var builder = new EnrollmentUseCaseBuilder();
        var leavingEnrollment = builder.ExistingEnrollment(TestData.Today.AddDays(-30));
        leavingEnrollment.End(TestData.Today);
        builder.Enrollments
            .Setup(repository => repository.FindCurrentAsync(builder.Student.Id, builder.ClassGroup.Id, TestData.Today, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leavingEnrollment);
        builder.Enrollments
            .Setup(repository => repository.GetForUpdateAsync(leavingEnrollment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leavingEnrollment);

        var response = await builder.BuildEnroll().ExecuteAsync(builder.ValidCommand(), CancellationToken.None);

        response.Value!.EndDate.ShouldBeNull();
    }
}
